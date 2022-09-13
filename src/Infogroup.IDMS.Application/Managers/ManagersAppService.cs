using Infogroup.IDMS.Databases;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Managers.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Abp.UI;
using Infogroup.IDMS.Contacts;
using Infogroup.IDMS.Common.Exporting;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Managers.Exporting;
using System.Collections.Generic;
using System.Data.SqlClient;
using Infogroup.IDMS.MasterLoLs;
using Infogroup.IDMS.ListCASContacts;
using Infogroup.IDMS.Sessions;

namespace Infogroup.IDMS.Managers
{
    [AbpAuthorize(AppPermissions.Pages_Managers)]
    public class ManagersAppService : IDMSAppServiceBase, IManagersAppService
    {

        private readonly ICommonExcelExporter _commonExcelExporter;
        private readonly IDatabaseRepository _customDatabaseRepository;
        private readonly IContactRepository _customContactRepository;
        private readonly IManagerRepository _customManagerRepository;
        private readonly IManagersExcelExporter _managersExcelExporter;
        private readonly IRepository<MasterLoL, int> _masterLolRepository;
        private readonly IRepository<ListCASContact, int> _listCasContactRepository;
        private readonly AppSession _mySession;

        public ManagersAppService(
             AppSession mySession,
            ICommonExcelExporter commonExcelExporter,
            IDatabaseRepository customDatabaseRepository,
            IContactRepository customContactRepository,

            IManagerRepository customManagerRepository,
            IManagersExcelExporter managersExcelExporter,
            IRepository<MasterLoL, int> masterLolRepository,
            IRepository<ListCASContact, int> listCasContactRepository)
        {
            _commonExcelExporter = commonExcelExporter;
            _customDatabaseRepository = customDatabaseRepository;
            _customContactRepository = customContactRepository;
            _customManagerRepository = customManagerRepository;

            _managersExcelExporter = managersExcelExporter;
            _masterLolRepository = masterLolRepository;
            _listCasContactRepository = listCasContactRepository;
            _mySession = mySession;
        }

        #region Fetch Managers
        public PagedResultDto<ManagerDto> GetAllManagers(GetAllSetupInput input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var query = GetAllManagersQuery(input);
                var result = _customManagerRepository.GetAllManagersList(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CreateOrEditManagerDto> GetManagerForEdit(EntityDto input)
        {
            try
            {
                var manager = await _customManagerRepository.FirstOrDefaultAsync(input.Id);
                var output = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditManagerDto>(manager));
                return output;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region Save Managers
        [AbpAuthorize(AppPermissions.Pages_Managers_Create, AppPermissions.Pages_Managers_Edit)]
        public async Task CreateOrEdit(CreateOrEditManagerDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                ValidateManagers(input);
                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var manager = ObjectMapper.Map<Manager>(input);
                    await _customManagerRepository.InsertAsync(manager);
                }
                else
                {
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    var updateManager = _customManagerRepository.Get(input.Id.GetValueOrDefault());
                    ObjectMapper.Map(input, updateManager);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Validation
        private void ValidateManagers(CreateOrEditManagerDto input)
        {
            var isExistingCodeCount = 0;
            var isExistingCompanyCount = 0;
            if (!string.IsNullOrWhiteSpace(input.cCode))
            {

                isExistingCodeCount = _customManagerRepository.GetAll().Count(x => x.DatabaseId == input.DatabaseId && x.Id != input.Id && x.cCode.Trim() == input.cCode.Trim());
                if (isExistingCodeCount > 0) throw new UserFriendlyException(L("ValidateCode"));
            }
            if (!string.IsNullOrWhiteSpace(input.cCompany))
            {
                isExistingCompanyCount = _customManagerRepository.GetAll().Count(x => x.DatabaseId == input.DatabaseId && x.Id != input.Id && x.cCompany.Trim() == input.cCompany.Trim());
                if (isExistingCompanyCount > 0) throw new UserFriendlyException(L("ValidateCompany"));
            }
        }
        #endregion

        #region Print Managers and Contact Assignments

        [AbpAuthorize(AppPermissions.Pages_Managers_Print)]
        public FileDto ExportManagersToExcel(GetAllSetupInput input)
        {
            try
            {
                var managerList = GetAllManagers(input);
                var excelData = managerList.Items.ToList().Select(manager =>
                {
                    var managerExcelDto = ObjectMapper.Map<ExcelExporterDto>(manager);
                    managerExcelDto.ContactsList = _customContactRepository.GetContacts(manager.Id, ContactType.Manager, ContactTableType.TBLMANAGER);
                    return managerExcelDto;
                }).ToList();

                var databaseName = _customDatabaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseName")}: {databaseName}";
                var fileName = $"{L("Manager")}";

                return _commonExcelExporter.AllExportToFile(excelData, databaseName, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Managers_PrintContactAssignments)]
        public FileDto ExportContactAssignmentsToExcel(GetAllSetupInput filters)
        {
            try
            {
                var query = GetAllManagersQuery(filters, true);
                var contactAssignmentData = _customManagerRepository.GetContactAssignmentsData(query.Item1, query.Item3);
                var OrderList = _masterLolRepository.GetAll().Where(x => x.DatabaseId == filters.SelectedDatabase && x.iIsActive);
                var CasContactList = _listCasContactRepository.GetAll();
                contactAssignmentData.ForEach(p =>
                {
                    p.OrderList = OrderList.Where(x => x.iOrderContactID == p.ContactId).OrderBy(x => x.cListName).Select(x => $"{x.cListName}({x.Id})").ToList();
                    p.Dwap = (from order in OrderList
                              join casContact in CasContactList
                              on order.Id equals casContact.ListID
                              where casContact.ContactID.Equals(p.ContactId)
                              orderby order.cListName
                              select $"{order.cListName}({order.Id})").ToList();
                });

                var databaseName = _customDatabaseRepository.Get(filters.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseName")}:{databaseName}";
                var fileName = $"{L("DownloadAssignments")}";

                return _managersExcelExporter.ExportToFile(contactAssignmentData, databaseName, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Querybuilder
        private Tuple<string, string, List<SqlParameter>> GetAllManagersQuery(GetAllSetupInput filters, bool isContactsAssigmentExport = false)
        {
            try
            {
                string[] filtersarray = null;
                var isOrderId = Validation.ValidationHelper.IsNumeric(filters.Filter);
                if (!string.IsNullOrEmpty(filters.Filter))
                {
                    filtersarray = filters.Filter.Split(',');
                }

                var query = new Common.QueryBuilder();
                var codeandCompanyFilter = $@"AND (O.CCODE LIKE @FilterText OR O.CCOMPANY LIKE @FilterText)";
                if (isContactsAssigmentExport)
                    query.AddSelect("O.cCompany AS [List Manager], cFirstName + ' ' +  cLastName + '('  + cEmailAddress + ')' AS[Name], C.ID");
                else
                    query.AddSelect($"O.ID, O.CCODE, O.CCOMPANY, O.CCity, O.cAddress1, O.cAddress2, O.CSTATE, O.cPhone, O.cFax, O.cZip, (SELECT COUNT(*) FROM TBLCONTACT CCOUNT WHERE CCOUNT.CONTACTID = O.ID AND CCOUNT.CTYPE = {Convert.ToInt32(ContactType.Manager).ToString()}) AS contactsCount,STUFF(ISNULL(',' + nullif(O.CCity,''), '') + ISNULL(',' + nullif( O.CSTATE,''), '') + ISNULL(' ' + nullif(O.CZIP, ''),''),1,1,'') AS Address,O.IISACTIVE");
                query.AddFrom("TBLMANAGER", "O");


                if (isContactsAssigmentExport)
                {
                    query.AddJoin("TBLCONTACT", "C", "ID", "O", "INNER JOIN", "CONTACTID").And("C.CTYPE", "EQUALTO", Convert.ToInt32(ContactType.Manager).ToString());
                    query.AddJoin("tblMasterLoL", "ML", "ID", "O", "INNER JOIN", "ManagerID").And("ML.iIsActive", "EQUALTO", "1");
                    query.AddWhere("", "(", "EXISTS", "SELECT 1 FROM tblListCASContact LC inner join tblmasterlol ml on LC.ListID = ml.ID and ml.iIsActive = 1 WHERE LC.ContactID = C.ID");
                    query.AddWhere("OR", "", "EXISTS", "SELECT 1 FROM tblMasterLoL WHERE iOrderContactID = C.ID AND iIsActive = 1)");

                }
                else
                    query.AddJoin("TBLCONTACT", "C", "ID", "O", "LEFT JOIN", "CONTACTID").And("C.CTYPE", "EQUALTO", Convert.ToInt32(ContactType.Manager).ToString());

                query.AddWhere("And", "O.DatabaseID", "EQUALTO", filters.SelectedDatabase.ToString());

                if (isOrderId)
                    query.AddWhere("AND", "O.ID", "IN", filtersarray);
                else
                    query.AddWhereString(codeandCompanyFilter);
                query.AddWhere("AND", "O.iIsActive", "EQUALTO", filters.iIsActiveFilter.ToString());
                query.AddWhere("AND", "C.cLastName", "LIKE", filters.ContactLastNameFilterText);
                query.AddWhere("AND", "C.cEmailAddress", "LIKE", filters.ContactEmailFilterText);

                if (isContactsAssigmentExport)
                {
                    query.AddGroupBy(" O.cCompany,cFirstName, cLastName, cEmailAddress, C.ID");
                    query.AddHaving("", "COUNT(C.ID)", "GREATERTHAN", "0");
                    query.AddSort("[List Manager], [name]");
                }
                else
                {
                    query.AddSort(filters.Sorting ?? "cCompany ASC");
                    query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
                }
                query.AddDistinct();

                (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
                sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

                var sqlCount = query.BuildCount().Item1;
                return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}