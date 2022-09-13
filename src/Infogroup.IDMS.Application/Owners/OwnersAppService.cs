using Infogroup.IDMS.Databases;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Infogroup.IDMS.Owners.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Abp.UI;
using Infogroup.IDMS.Contacts;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Common.Exporting;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
using System.Data.SqlClient;
using Abp.Domain.Repositories;

namespace Infogroup.IDMS.Owners
{
    [AbpAuthorize(AppPermissions.Pages_Owners)]
    public class OwnersAppService : IDMSAppServiceBase, IOwnersAppService
    {
        private readonly IRepository<Database, int> _databaseRepository;
        private readonly AppSession _mySession;
        private readonly ICommonExcelExporter _commonExcelExporter;
        private readonly IContactRepository _customContactRepository;
        private readonly IOwnerRepository _customOwnerRepository;


        public OwnersAppService(
            IRepository<Database, int> databaseRepository,
            AppSession mySession,
            ICommonExcelExporter commonExcelExporter,
            IContactRepository customContactRepository,
            IOwnerRepository customOwnerRepository
            )
        {
            _databaseRepository = databaseRepository;
            _customOwnerRepository = customOwnerRepository;
            _mySession = mySession;
            _commonExcelExporter = commonExcelExporter;
            _customContactRepository = customContactRepository;
        }

        #region Fetch Owners
        public PagedResultDto<OwnerDto> GetAllOwners(GetAllSetupInput input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var query = GetAllOwnerQuery(input);
                var result = _customOwnerRepository.GetAllOwnersList(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task<CreateOrEditOwnerDto> GetOwnerForEdit(EntityDto input)
        {
            try
            {
                var owner = await _customOwnerRepository.FirstOrDefaultAsync(input.Id);
                var editOwnerData = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditOwnerDto>(owner));
                return editOwnerData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Save Owners
        [AbpAuthorize(AppPermissions.Pages_Owners_Create, AppPermissions.Pages_Owners_Edit)]
        public async Task CreateOrEdit(CreateOrEditOwnerDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                ValidateOwners(input);
                if (input.Id == null)
                {

                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var owner = ObjectMapper.Map<Owner>(input);
                    await _customOwnerRepository.InsertAsync(owner);
                }
                else
                {
                    var updateOwner = _customOwnerRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updateOwner);
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
        private void ValidateOwners(CreateOrEditOwnerDto input)
        {

            if (!string.IsNullOrWhiteSpace(input.cCode))
            {
                var isExistingCodeCount = 0;
                isExistingCodeCount = _customOwnerRepository.GetAll().Count(p => p.DatabaseId == input.DatabaseId && p.cCode.Trim() == input.cCode.Trim() && p.Id != input.Id);
                if (isExistingCodeCount > 0) throw new UserFriendlyException(L("ValidateCode"));
            }
            if (!string.IsNullOrWhiteSpace(input.cCompany))
            {
                var isExistingCompanyCount = 0;
                isExistingCompanyCount = _customOwnerRepository.GetAll().Count(p => p.DatabaseId == input.DatabaseId && p.cCompany == input.cCompany && p.Id != input.Id);
                if (isExistingCompanyCount > 0) throw new UserFriendlyException(L("ValidateCompany"));
            }
        }
        #endregion

        #region Print Owners

        [AbpAuthorize(AppPermissions.Pages_Owners_Print)]
        public FileDto ExportToExcel(GetAllSetupInput input)
        {
            try
            {
                var ownerList = GetAllOwners(input);
                var excelData = ownerList.Items.ToList().Select(owner =>
                {
                    var ownerExcelDto = ObjectMapper.Map<ExcelExporterDto>(owner);
                    ownerExcelDto.ContactsList = _customContactRepository.GetContacts(owner.Id, ContactType.Owner, ContactTableType.TBLOWNER);
                    return ownerExcelDto;
                }).ToList();
                var databaseName = _databaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseName")}:{databaseName}";
                var fileName = $"{L("Owner")}";

                return _commonExcelExporter.AllExportToFile(excelData, databaseName, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Owners Bizness
        private static Tuple<string, string, List<SqlParameter>> GetAllOwnerQuery(GetAllSetupInput filters)
        {

            string[] filtersarray = null;
            var isOrderId = Validation.ValidationHelper.IsNumeric(filters.Filter);
            if (!string.IsNullOrEmpty(filters.Filter))
            {
                filtersarray = filters.Filter.Split(',');
            }

            var codeandCompanyFilter = $@"AND (O.CCODE LIKE @FilterText OR O.CCOMPANY LIKE @FilterText)";

            var query = new Common.QueryBuilder();
            query.AddSelect($"O.ID, O.CCODE, O.CCOMPANY, O.CCity, O.cAddress1, O.cAddress2, O.CSTATE, O.cPhone, O.cFax, O.cZip, (SELECT COUNT(*) FROM TBLCONTACT CCOUNT WHERE CCOUNT.CONTACTID = O.ID AND CCOUNT.CTYPE = {Convert.ToInt32(ContactType.Owner).ToString()}) AS contactsCount,STUFF(ISNULL(',' + nullif(O.CCity,''), '') + ISNULL(',' + nullif( O.CSTATE,''), '') + ISNULL(' ' + nullif(O.CZIP, ''),''),1,1,'') AS Address,O.IISACTIVE");
            query.AddFrom("TBLOWNER", "O");
            query.AddJoin("TBLCONTACT", "C", "ID", "O", "LEFT JOIN", "CONTACTID").And("C.CTYPE", "EQUALTO", Convert.ToInt32(ContactType.Owner).ToString());
            query.AddWhere("And", "O.DatabaseID", "EQUALTO", filters.SelectedDatabase.ToString());

            if (isOrderId)
                query.AddWhere("AND", "O.ID", "IN", filtersarray);
            else
            {
                query.AddWhereString(codeandCompanyFilter);
            }
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

    }
}