using Abp.UI;
using Infogroup.IDMS.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.ShippedReports.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Campaigns;
using Syncfusion.XlsIO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Infogroup.IDMS.Configuration;
using Infogroup.IDMS.Dto;
using System.IO;
using Abp.AspNetZeroCore.Net;


namespace Infogroup.IDMS.ShippedReports
{
    public class ShippedReportAppService : IDMSAppServiceBase,IShippedReportAppService
    {    
        private readonly AppSession _mySession;
        private readonly IRedisIDMSUserCache _userCache;
        private readonly ICampaignRepository _customCampaignRepository;
        private readonly IShortSearch _shortSearch;
        private readonly string _webRootPath;
        private readonly IConfigurationRoot _appConfiguration;
        const string _fileName = "ShippedReport.xlsx";


        public ShippedReportAppService(
            IRedisIDMSUserCache userCache,
            AppSession mySession,
            IShortSearch shortSearch,
            ICampaignRepository campaignRepository,
            IHostingEnvironment env
            )
        {
             _userCache = userCache;
             _shortSearch = shortSearch;
            _customCampaignRepository = campaignRepository;
            _mySession= mySession ;
            _appConfiguration = env.GetAppConfiguration();
            _webRootPath = env.WebRootPath;
        }

        #region Fetch Shipped Report
        public PagedResultDto<GetShippedReportView> GetAllShippedReports(GetShippedReportInput input)
        {
            try
            {
                var databaseIds = _userCache.GetDatabaseIDs(_mySession.IDMSUserId);
                var shortWhere = _shortSearch.GetWhere(PageID.ShippedReports, input.Filter);
                var query=GetAllShippedReportsQuery(input, _mySession.IDMSUserId, _mySession.IDMSUserName, databaseIds, shortWhere);
                return _customCampaignRepository.GetAllShippedReportsList(query);
            
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private static Tuple<string, string, List<SqlParameter>> GetAllShippedReportsQuery(GetShippedReportInput filters, int CurrentUserId,string userName, List<int> DatabaseIds, string shortWhere)
        {

            if (!string.IsNullOrEmpty(shortWhere) && shortWhere.Length > 0)
                filters.Filter = string.Empty;

            if (!string.IsNullOrEmpty(filters.Filter))
                filters.Filter = filters.Filter.Trim();

            string[] filtersarray = null;
            var isShippedId = Validation.ValidationHelper.IsNumeric(filters.Filter);

            var DbIds = DatabaseIds.Select(x => x.ToString()).ToArray();

            if (!string.IsNullOrEmpty(filters.Filter))
                filtersarray = filters.Filter.Split(',');

            var date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var query = new QueryBuilder();
            query.AddSelect($"db.cDatabaseName, CASE WHEN o.DivisionMailerID = 0 THEN M.cCompany ELSE dm.cCompany END AS cCompany,CASE WHEN o.DivisionBrokerID = 0 THEN BR.cCompany ELSE DBR.cCompany END AS Broker,CASE WHEN iIsNetUse = 1 THEN 'Y' ELSE '' END AS NetOrder, CASE WHEN iIsNoUsage = 1 THEN 'Y' ELSE '' END AS NetUsage, O.cBrokerPONo, o.ID, o.cLVAOrderNo, O.cDecoyKey, o.dShipDateShipped, O.iProvidedCount, U.cFirstName +' '+ U.cLastName as cCreatedby, U1.cFirstName +' '+ U1.cLastName as ShippedBy,O.ID as OrderID, O.cDescription as OrderDescription, O.cExportLayout as ExportLayoutName,F.LK_OfferType,OB.iOESSInvoiceTotal,OB.cOESSInvoiceNumber");
            query.AddFrom("tblorder", "O");
            query.AddJoin("tblOffer", "F", "OfferID", "O", "INNER JOIN", "ID");
            query.AddJoin("tblMailer", "M", "MailerID", "O", "INNER JOIN", "ID");
            query.AddJoin("tblGroupBroker", "GB", "BrokerID", "M");
            query.AddJoin("tblUserGroup", "UG", "GroupID", "GB");
            query.AddJoin("tblOrderStatus", "OS", "ID", "O", "INNER JOIN", "OrderID").And("os.iIsCurrent", "EQUALTO", "1").And("os.iStatus", "EQUALTO", "130");
            query.AddJoin("tblUser", "U", "cCreatedBy", "O", "INNER JOIN", "cUserID");
            query.AddJoin("tblUser", "U1", "cCreatedBy", "OS", "INNER JOIN", "cUserID");
            query.AddJoin("tblBuild", "B", "BuildID", "O", "INNER JOIN", "ID").And("B.DatabaseID", "EQUALTO", "M.DatabaseID");
            query.AddJoin("tblDatabase", "DB", "DatabaseID", "B", "INNER JOIN", "ID").And("B.DatabaseID", "EQUALTO", "M.DatabaseID");
            query.AddJoin("tblDivisionMailer", "DM", "DivisionMailerID", "O", "LEFT JOIN", "ID");
            query.AddJoin("tblBroker", "BR", "BrokerID", "M", "INNER JOIN", "ID");
            query.AddJoin("tblDivisionBroker", "DBR", "DivisionBrokerID", "O", "LEFT JOIN", "ID");
            query.AddJoin("tblOrderBilling", "OB", "ID", "O", "LEFT JOIN", "OrderID");
            query.AddWhere("", "UG.UserID", "EQUALTO", CurrentUserId.ToString());
            query.AddWhere("AND", "B.DatabaseID", "IN", DbIds);
            if(!shortWhere.Contains("o.dShipDateShipped"))
            query.AddWhere("AND", "O.dShipDateShipped", "BETWEEN", new string[] { firstDayOfMonth.ToString(), lastDayOfMonth.ToString() });
            if (isShippedId)
                query.AddWhere("AND", "O.ID", "IN", filtersarray);
            else
                query.AddWhere("AND", "db.cDatabaseName", "LIKE", filters.Filter);

            if (shortWhere.Length > 0)
                query.AddWhereString($"AND ({shortWhere})");

            query.AddSort(filters.Sorting ?? "'dShipDateShipped' DESC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        #endregion

        #region Download ShippedReports

        public FileDto DownloadShippedReport(GetShippedReportInput input)
        {
            try
            {
                var _filePath = $"{_webRootPath}/{_fileName}";          
                var shippedReportList = GetAllShippedReports(input);
                var excelData = shippedReportList.Items.ToList();
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    var application = excelEngine.Excel;
                    var workbook = application.Workbooks.Create(1);
                    workbook.Version = ExcelVersion.Excel2016;
                    var dataSheet = workbook.Worksheets[0];
                    dataSheet.Name = $"{L("ShippedReport")}";
                
                    var rows = excelData.Count;
                    dataSheet.ImportData(excelData,1, 1, true);
                    if (rows > 0)
                    {
                        dataSheet.Range[$"I2:I{rows + 1}"].NumberFormat = "###,##";
                        dataSheet.Range[$"J1:J{rows}"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        dataSheet.Range[$"K1:K{rows}"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        dataSheet.Range[$"L1:L{rows}"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    }
                    dataSheet.UsedRange[1, 1, 1, 18].CellStyle.ColorIndex = ExcelKnownColors.Yellow;
                    dataSheet.UsedRange.AutofitColumns();
                    dataSheet.UsedRange.AutofitRows();
                    



                    using (FileStream fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        workbook.SaveAs(fileStream);
                        workbook.Close();
                    }
                    excelEngine.ThrowNotSavedOnDestroy = false;
                    var fileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet;
                    return new FileDto(_filePath, fileType, true);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

    }
}