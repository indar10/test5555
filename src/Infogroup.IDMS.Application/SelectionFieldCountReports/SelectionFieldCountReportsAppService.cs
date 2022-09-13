using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Common.Exporting;
using Infogroup.IDMS.Configuration;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.SelectionFieldCountReports.Dtos;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.ShortSearch;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Infogroup.IDMS.SelectionFieldCountReports
{
    public class SelectionFieldCountReportsAppService: IDMSAppServiceBase,ISelectionFieldCountReportAppService
    {                
        private readonly ICampaignRepository _customCampaignRepository;
        private readonly IShortSearch _shortSearch;                
        const string _fileName = "SelectionFieldCountReport.xlsx";       
        private readonly ICommonExcelExporter _commonExcelExporter;
        private readonly IRepository<Database, int> _databaseRepository;


        public SelectionFieldCountReportsAppService(
            IRedisIDMSUserCache userCache,
            AppSession mySession,
            IShortSearch shortSearch,
            ICampaignRepository campaignRepository,
            IHostingEnvironment env,            
            ICommonExcelExporter commonExcelExporter,
            IRepository<Database, int> databaseRepository
            )
        {            
            _shortSearch = shortSearch;
            _customCampaignRepository = campaignRepository;                       
            _commonExcelExporter = commonExcelExporter;
            _databaseRepository = databaseRepository;
        }

        #region Fetch Selection Field Count Report
        public PagedResultDto<GetSelectionFieldCountReportView> GetAllSelectionFieldCountReports(GetSelectionFieldCountReportInput input)
        {
            try
            {               
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;                
                var shortWhere = _shortSearch.GetWhere(PageID.SelectionFieldCountReports, input.Filter);
                if (!string.IsNullOrEmpty(shortWhere)) { 
                var query = GetAllSelectionFieldCountReportsQuery(input,shortWhere);                
                return _customCampaignRepository.GetAllSelectionFieldCountReportsList(query);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Selection Field Count Report Bizness
        private static Tuple<string, string, List<SqlParameter>> GetAllSelectionFieldCountReportsQuery(GetSelectionFieldCountReportInput filters, string shortWhere)
        {
           
            if (!string.IsNullOrEmpty(shortWhere) && shortWhere.Length > 0)
                filters.Filter = string.Empty;

            if (!string.IsNullOrEmpty(filters.Filter))
                filters.Filter = filters.Filter.Trim();
            
            var query = new QueryBuilder();            
            query.AddSelect($"S.cQuestionFieldName,LK.cDescription,OS.iStatus,count(DISTINCT O.ID) as 'Count'");
            query.AddFrom("tblorder", "O"); 
            query.AddJoin("tblBuild", "B", "BuildID", "O", "INNER JOIN", "ID");
            query.AddJoin("tblDatabase", "DB", "DatabaseID", "B", "INNER JOIN", "ID");
            query.AddJoin("tblSegment", "SEG", "ID", "O", "INNER JOIN", "OrderID");         
            query.AddJoin("tblSegmentSelection", "S", "ID","SEG" ,"INNER JOIN", "SegmentID");
            query.AddJoin("tblOrderStatus", "OS", "ID", "O", "INNER JOIN", "OrderID").And("OS.iIsCurrent","EQUALTO","1");
            query.AddJoin("tblLookup", "LK", "iStatus", "OS", "INNER JOIN", "cCode").And("LK.cLookupValue", "EQUALTO", "'ORDERLOADSTATUS'");
            query.AddWhere("", "DB.ID", "EQUALTO", filters.SelectedDatabase.ToString());
            if(!string.IsNullOrEmpty(filters.SelectedcQuestionFieldName))
            {
                query.AddWhere("AND", "S.cQuestionFieldName", "LIKE", filters.SelectedcQuestionFieldName);
                query.AddWhere("AND", "OS.iStatus", "EQUALTO", filters.SelectediStatus);
            }                    
            if (shortWhere.Length > 0)
                query.AddWhereString($"AND ({shortWhere})");
                                  
            query.AddGroupBy("S.cQuestionFieldName,LK.cDescription,OS.iStatus");
            query.AddSort(filters.Sorting ?? "S.cQuestionFieldName ASC");           

            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCountSelectionFieldCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        #endregion

        #region Get expanded row records
      
       public PagedResultDto<GetOrderDetailsView> GetOrderDetails(GetOrderDetailInput input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var shortWhere = _shortSearch.GetWhere(PageID.SelectionFieldCountReports, input.Filter);
                var query = GetOrderDetailsQuery(input,shortWhere);
                return _customCampaignRepository.GetOrderDetailsList(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Expanded row records bizness
        private static Tuple<string, string, List<SqlParameter>> GetOrderDetailsQuery(GetOrderDetailInput filters,string shortWhere)
        {

            if (!string.IsNullOrEmpty(shortWhere) && shortWhere.Length > 0)
                filters.Filter = string.Empty;
            if (!string.IsNullOrEmpty(filters.Filter))
                filters.Filter = filters.Filter.Trim();
            var query = new Common.QueryBuilder();
            query.AddSelect($"O.ID,O.cDescription,O.iProvidedCount,OS.dCreatedDate,U.cFirstName + ' ' + U.cLastName As [CreatedBy]");
            query.AddFrom("tblorder", "O");
            query.AddJoin("tblOrderStatus", "OS", "ID", "O", "INNER JOIN", "OrderID").And("OS.iIsCurrent", "EQUALTO", "1");
            query.AddJoin("tblBuild", "B", "BuildID", "O", "INNER JOIN", "ID");
            query.AddJoin("tblDatabase", "DB", "DatabaseID", "B", "INNER JOIN", "ID");
            query.AddJoin("tblSegment", "SEG", "ID", "O", "INNER JOIN", "OrderID");
            query.AddJoin("tblSegmentSelection", "S", "ID", "SEG", "INNER JOIN", "SegmentID");
            query.AddJoin("tblUser", "U", "cCreatedBy", "O", "INNER JOIN", "cUserID");
            query.AddWhere("", "DB.ID", "EQUALTO", (filters.SelectedDatabase).ToString());
            query.AddWhere("AND", "OS.iStatus", "EQUALTO", (filters.iStatus).ToString());
            query.AddWhere("AND", "S.cQuestionFieldName", "LIKE", filters.cQuestionFieldName);
            if (shortWhere.Length > 0)
                query.AddWhereString($"AND ({shortWhere})");
            query.AddSort(filters.Sorting ?? "O.ID DESC");
            if (filters.DownloadFlag) { 
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            }
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();            

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        #endregion

        #region Download Selection Field Count Report       
        public FileDto DownloadSelectionFieldCountReport(GetSelectionFieldCountReportInput input)
        {           
            try
            {
                System.Reflection.PropertyInfo pi = input.GetType().GetProperty("Filter");
                string shortFilter = (string)pi.GetValue(input, null);
                var selectionFieldCountReportList = GetAllSelectionFieldCountReports(input);
                string filter = input.Filter;
                var excelData = new List<GetSelectionFieldCountReportView>();
                if (selectionFieldCountReportList != null) { 
                    excelData = selectionFieldCountReportList.Items.ToList().Select(record =>
                    {
                        GetOrderDetailInput orderDetailsInput = new GetOrderDetailInput();
                        orderDetailsInput.cQuestionFieldName = record.cQuestionFieldName;
                        orderDetailsInput.iStatus = record.iStatus;
                        orderDetailsInput.SelectedDatabase = input.SelectedDatabase;
                        orderDetailsInput.Filter = shortFilter;
                        orderDetailsInput.DownloadFlag = false;
                        record.OrderDetailsList = GetOrderDetails(orderDetailsInput).Items.ToList();
                        return record;
                    }).ToList();
                }

                var databaseName = _databaseRepository.Get(input.SelectedDatabase).cDatabaseName;
                databaseName = $"{L("DatabaseName")}:{databaseName}";
                var fileName = $"{L("SelectionFieldCount")}";
                return _commonExcelExporter.SelectionFieldCountAllExportToFile(excelData, databaseName, fileName);
                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }


}
