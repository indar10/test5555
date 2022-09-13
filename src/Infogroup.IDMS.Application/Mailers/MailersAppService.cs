using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Mailers.Exporting;
using Infogroup.IDMS.Mailers.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Infogroup.IDMS.Contacts;
using Abp.UI;
using Infogroup.IDMS.Brokers;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Common.Exporting;
using Infogroup.IDMS.Offers;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Shared.Dtos;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Mailers
{
    [AbpAuthorize(AppPermissions.Pages_Mailers)]
    public class MailersAppService : IDMSAppServiceBase, IMailersAppService
    {
        private readonly IMailerRepository _customMailerRepository;
        private readonly IMailersExcelExporter _mailersExcelExporter;
        private readonly IRepository<Broker, int> _brokerRepository;
        private readonly IDatabaseRepository _customDatabaseRepository;
        private readonly ICommonExcelExporter _commonExcelExporter;
        private readonly IContactRepository _customContactRepository;
        private readonly IOfferRepository _customOfferRepository;
        private readonly AppSession _mySession;
        private readonly OfferDomainService _offerDomainService;


        public MailersAppService(IMailerRepository customMailerRepository,
            IMailersExcelExporter mailersExcelExporter,
            IRepository<Broker, int> brokerRepository,
            IDatabaseRepository customDatabaseRepository,
            ICommonExcelExporter commonExcelExporter,
            IContactRepository customContactRepository,
            IOfferRepository customOfferRepository,
            AppSession mySession,
            OfferDomainService offerDomainService)
        {
            _customMailerRepository = customMailerRepository;
            _mailersExcelExporter = mailersExcelExporter;
            _brokerRepository = brokerRepository;
            _customDatabaseRepository = customDatabaseRepository;
            _commonExcelExporter = commonExcelExporter;
            _customContactRepository = customContactRepository;
            _customOfferRepository = customOfferRepository;
            _mySession = mySession;
            _offerDomainService = offerDomainService;
        }

        #region Fetch Mailers
        public PagedResultDto<MailerDto> GetAllMailers(GetAllSetupInput input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var query = GetAllMailerQuery(input);
                var result = _customMailerRepository.GetAllMailersList(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CreateOrEditMailerDto> GetMailerForEdit(EntityDto input)
        {
            try
            {
                var mailer = await _customMailerRepository.FirstOrDefaultAsync(input.Id);
                var editMailerData = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditMailerDto>(mailer));
                return editMailerData;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Save Offers
        [AbpAuthorize(AppPermissions.Pages_Mailers_Create, AppPermissions.Pages_Mailers_Edit)]
        public async Task CreateOrEdit(CreateOrEditMailerDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                ValidateMailers(input);
                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var mailer = ObjectMapper.Map<Mailer>(input);
                    var mailerId = await _customMailerRepository.InsertAndGetIdAsync(mailer);

                    if (input.AddPostalOffer)
                        AddGenericOffer(input.AutoApproveAllOffer, mailerId, MailerConsts.Postal);
                    if (input.AddTelemarketingOffer)
                        AddGenericOffer(input.AutoApproveAllOffer, mailerId, MailerConsts.Telemarketing);
                }
                else
                {
                    var updateMailer = _customMailerRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updateMailer);
                    CurrentUnitOfWork.SaveChanges();

                    if (!input.iIsActive)
                    {
                        var offers = _customOfferRepository.GetAll().Where(t => t.MailerId == input.Id).ToList();
                        if (offers.Count > 0)
                        {
                            foreach (var offer in offers)
                            {
                                offer.iIsActive = input.iIsActive;
                                offer.cModifiedBy = input.cModifiedBy;
                                offer.dModifiedDate = DateTime.Now;
                                _customOfferRepository.Update(offer);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private void AddGenericOffer(bool autoApproveAllOffer, int mailerId, string lK_OfferType)
        {
            try
            {
                var offer = new Offer
                {
                    MailerId = mailerId,
                    LK_OfferType = lK_OfferType,
                    cOfferName = MailerConsts.GenericOffer,
                    cOfferCode = string.Empty,
                    iIsActive = true,
                    cCreatedBy = _mySession.IDMSUserName,
                    dCreatedDate = DateTime.Now
                };

                _offerDomainService.AddOffer(offer, autoApproveAllOffer);
                return;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Validation
        private void ValidateMailers(CreateOrEditMailerDto input)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(input.cCode))
                {
                    var isExistingCodeCount = 0;
                    isExistingCodeCount = _customMailerRepository.GetAll().Count(p => p.DatabaseID == input.DatabaseId && p.cCode.Trim() == input.cCode.Trim() && p.Id != input.Id);
                    if (isExistingCodeCount > 0) throw new UserFriendlyException(L("ValidateCode"));
                }
                if (!string.IsNullOrWhiteSpace(input.cCompany))
                {
                    var isExistingCompanyCount = 0;
                    isExistingCompanyCount = _customMailerRepository.GetAll().Count(p => p.DatabaseID == input.DatabaseId && p.cCompany.Trim() == input.cCompany.Trim() && p.Id != input.Id);
                    if (isExistingCompanyCount > 0) throw new UserFriendlyException(L("ValidateCompany"));
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);

            }
        }
        #endregion

        #region Print Mailers and Offers
        [AbpAuthorize(AppPermissions.Pages_Mailers_Print)]
        public FileDto ExportAllMailerToExcel(GetAllSetupInput input)
        {
            try
            {
                var mailerList = GetAllMailers(input);
                var excelData = mailerList.Items.ToList().Select(mailer =>
                {
                    var mailerExcelDto = ObjectMapper.Map<ExcelExporterDto>(mailer);
                    mailerExcelDto.ContactsList = _customContactRepository.GetContacts(mailer.Id, ContactType.Mailer, ContactTableType.TBLMAILER);
                    return mailerExcelDto;
                }).ToList();

                var databaseName = _customDatabaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseNameForExcel")}{databaseName}";
                var fileName = $"{L("Mailer")}";

                return _commonExcelExporter.AllExportToFile(excelData, databaseName, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Mailers_PrintOffers)]
        public FileDto ExportOffers(GetAllSetupInput input)
        {
            try
            {
                var databaseName = _customDatabaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseNameForExcel")} {databaseName}";

                var mailerList = GetAllMailers(input);
                var mailerOffers = from mailer in mailerList.Items.ToList()
                                   join offer in _customOfferRepository.GetAll() on mailer.Id equals offer.MailerId
                                   where mailer.DatabaseId == input.SelectedDatabase &&
                                   mailer.iIsActive && offer.iIsActive
                                   orderby mailer.cCompany ascending
                                   select new MailerOfferDto
                                   {
                                       cCompany = mailer.cCompany,
                                       cOfferName = offer.cOfferName,
                                       cOfferCode = offer.cOfferCode,
                                       LK_OfferType = offer.LK_OfferType,
                                       iHideInDWAP = offer.iHideInDWAP ? MailerConsts.Yes : string.Empty
                                   };
                var fileName = $"{L("MailerOffer")}";

                return _mailersExcelExporter.ExportToFile(mailerOffers.ToList(), databaseName, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Mailers Bizness
        private Tuple<string, string, List<SqlParameter>> GetAllMailerQuery(GetAllSetupInput filters)
        {

            string[] filtersarray = null;
            var isOrderId = Validation.ValidationHelper.IsNumeric(filters.Filter);
            if (!string.IsNullOrEmpty(filters.Filter))
            {
                filtersarray = filters.Filter.Split(',');
            }

            var query = new Common.QueryBuilder();
            var codeandCompanyFilter = $@"AND (O.CCODE LIKE @FilterText OR O.CCOMPANY LIKE @FilterText OR B.CCOMPANY LIKE @FilterText)";
            query.AddSelect($"O.ID, O.DatabaseID, O.CCODE, B.CCOMPANY AS BROKER ,O.CCOMPANY, O.CCity, O.cAddress1, O.cAddress2, O.CSTATE, O.cPhone, O.cFax, O.cZip, (SELECT COUNT(*) FROM TBLCONTACT CCOUNT WHERE CCOUNT.CONTACTID = O.ID AND CCOUNT.CTYPE = {Convert.ToInt32(ContactType.Mailer).ToString()}) AS contactsCount,STUFF(ISNULL(',' + nullif(O.CCity,''), '') + ISNULL(',' + nullif( O.CSTATE,''), '') + ISNULL(' ' + nullif(O.CZIP, ''),''),1,1,'') AS Address,O.IISACTIVE");
            query.AddFrom("TBLMAILER", "O");
            query.AddJoin("TBLCONTACT", "C", "ID", "O", "LEFT JOIN", "CONTACTID").And("C.CTYPE", "EQUALTO", Convert.ToInt32(ContactType.Mailer).ToString());
            query.AddJoin("TBLBroker", "B", "BROKERID", "O", "LEFT JOIN", "ID");
            query.AddWhere("And", "O.DatabaseID", "EQUALTO", filters.SelectedDatabase.ToString());

            if (isOrderId)
                query.AddWhere("AND", "O.ID", "IN", filtersarray);
            else
                query.AddWhereString(codeandCompanyFilter);
            query.AddWhere("AND", "O.iIsActive", "EQUALTO", filters.iIsActiveFilter.ToString());
            query.AddWhere("AND", "C.cLastName", "LIKE", filters.ContactLastNameFilterText);
            query.AddWhere("AND", "C.cEmailAddress", "LIKE", filters.ContactEmailFilterText);
            query.AddSort(filters.Sorting ?? "cCompany ASC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        #endregion

        #region Mailers Dropdowns
        public List<DropdownOutputDto> GetAllBrokersbyDatabaseId(int databaseId)
        {
            try
            {
                return _brokerRepository.GetAll().Where(t => t.DatabaseID == databaseId && t.iIsActive)
                     .OrderBy(t => t.cCompany)
                     .Select(dv => new DropdownOutputDto
                     {
                         Value = dv.Id,
                         Label = $"{dv.cCode}  {dv.cCompany}"
                     }).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
    }
    #endregion
}