using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Infogroup.IDMS.Contacts;
using Abp.UI;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Common.Exporting;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Brokers.Dtos;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Infogroup.IDMS.Brokers
{
    [AbpAuthorize(AppPermissions.Pages_Brokers)]
    public class BrokersAppService : IDMSAppServiceBase, IBrokersAppService
    {       
        
        private readonly IBrokerRepository _customBrokerRepository;
        private readonly IDatabaseRepository _customDatabaseRepository;
        private readonly ICommonExcelExporter _commonExcelExporter;
        private readonly IContactRepository _customContactRepository;               
        private readonly AppSession _mySession;      
         

        public BrokersAppService(            
            IBrokerRepository customBrokerRepository,
            IDatabaseRepository customDatabaseRepository,
            ICommonExcelExporter commonExcelExporter,
            IContactRepository customContactRepository,  
            AppSession mySession
            )            
        { 
            _customBrokerRepository = customBrokerRepository;
            _customDatabaseRepository = customDatabaseRepository;
            _commonExcelExporter = commonExcelExporter;
            _customContactRepository = customContactRepository;           
            _mySession = mySession;
        }

        public PagedResultDto<BrokersDto> GetAllBrokers(GetAllBrokersInput input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var query = GetBrokers(input);
                var result = _customBrokerRepository.GetAllBrokersList(query.Item1, query.Item2, query.Item3, input.Filter);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CreateOrEditBrokerDto> GetBrokerForEdit(EntityDto input)
        {
            try
            {
                var broker = await _customBrokerRepository.FirstOrDefaultAsync(input.Id);
                var editBrokerData = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditBrokerDto>(broker));
                return editBrokerData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task CreateOrEdit(CreateOrEditBrokerDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                ValidateOwners(input);
                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var owner = ObjectMapper.Map<Broker>(input);
                    await _customBrokerRepository.InsertAsync(owner);
                }
                else
                {
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    var updateOwner = _customBrokerRepository.Get(input.Id.GetValueOrDefault());
                    ObjectMapper.Map(input, updateOwner);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        private void ValidateOwners(CreateOrEditBrokerDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.cCode))
            {
                var isExistingCodeCount = 0;
                isExistingCodeCount = _customBrokerRepository.GetAll().Count(p => p.DatabaseID == input.DatabaseID && p.cCode.Trim() == input.cCode.Trim() && p.Id != input.Id);
                if (isExistingCodeCount > 0) throw new UserFriendlyException(L("ValidateCode"));
            }
            if (!string.IsNullOrWhiteSpace(input.cCompany))
            {
                var isExistingCompanyCount = 0;
                isExistingCompanyCount = _customBrokerRepository.GetAll().Count(p => p.DatabaseID == input.DatabaseID && p.cCompany == input.cCompany && p.Id != input.Id);
                if (isExistingCompanyCount > 0) throw new UserFriendlyException(L("ValidateCompany"));
            }
        }
        [AbpAuthorize(AppPermissions.Pages_Brokers_Print)]
        public FileDto ExportToExcel(GetAllBrokersInput input)
        {
            try
            {
                var brokerList = GetAllBrokers(input);
                var excelData = brokerList.Items.ToList().Select(broker => 
                {
                 var brokerExcel = ObjectMapper.Map<ExcelExporterDto>(broker);
                 brokerExcel.ContactsList = _customContactRepository.GetContacts(broker.Id, ContactType.Broker, ContactTableType.TBLBROKER);
                return brokerExcel;
            }).ToList();

                var databaseName = _customDatabaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseName")}:{databaseName}";
                var fileName = $"{L("Broker")}";

                return _commonExcelExporter.AllExportToFile(excelData, databaseName, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        private Tuple<string, string, List<SqlParameter>> GetBrokers(GetAllBrokersInput filters)
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
                var codeandCompanyFilter = $@"AND (B.CCODE LIKE @Code OR B.CCOMPANY LIKE @Company)";
                query.AddSelect($"B.ID,   B.CCODE, B.CCOMPANY, B.CCity, B.cAddress1, B.cAddress2, B.CSTATE, B.cPhone, B.cFax, B.cZip,STUFF(ISNULL(',' + nullif(B.CCity,''), '') + ISNULL(',' + nullif( B.CSTATE,''), '') + ISNULL(' ' + nullif(B.CZIP, ''),''),1,1,'') AS Address,  (SELECT COUNT(*) FROM TBLCONTACT CCOUNT WHERE CCOUNT.CONTACTID  = B.ID AND CCOUNT.CTYPE = {Convert.ToInt32(ContactType.Broker).ToString()}) AS COUNTCONTACT, B.IISACTIVE ");
                query.AddFrom("TBLBROKER ", "B");
                query.AddJoin("TBLCONTACT", "C", "ID", "B", "LEFT JOIN", "CONTACTID").And("C.CTYPE", "EQUALTO", Convert.ToInt32(ContactType.Broker).ToString());
                query.AddWhere("And", "B.DatabaseID", "EQUALTO", filters.SelectedDatabase.ToString());

                if (isOrderId)
                    query.AddWhere("AND", "B.ID", "IN", filtersarray);
                else
                {
                    query.AddWhereString(codeandCompanyFilter);
                }
                query.AddWhere("AND", "B.iIsActive", "EQUALTO", filters.iIsActiveFilter.ToString());
                query.AddWhere("AND", "C.cLastName", "LIKE", filters.ContactLastNameFilterText);
                query.AddWhere("AND", "C.cEmailAddress", "LIKE", filters.ContactEmailFilterText);
                query.AddSort(filters.Sorting ?? "cCompany ASC");
                query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
                query.AddDistinct();
                (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
                var sqlCount = query.BuildCount().Item1;
                return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}