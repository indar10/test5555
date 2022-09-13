using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Campaigns.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Infogroup.IDMS.OrderExportParts.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.ShippedReports.Dtos;
using System.Data.Common;
using Infogroup.IDMS.SelectionFieldCountReports.Dtos;
using System.Linq;

namespace Infogroup.IDMS.Campaigns
{
    public class CampaignRepository : IDMSRepositoryBase<Campaign, int>, ICampaignRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public CampaignRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public async Task<PagedResultDto<GetCampaignsListForView>> GetAllCampaignsList(string selectQuery, string countQuery, List<SqlParameter> sqlParameters, GetCampaignListFilters filters, string userName)
        {
            _databaseHelper.EnsureConnectionOpen();
            try
            {
                var result = new PagedResultDto<GetCampaignsListForView>();

                using (var command = _databaseHelper.CreateCommand(countQuery, CommandType.Text, sqlParameters.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var items = new List<GetCampaignsListForView>();
                using (var command = _databaseHelper.CreateCommand(selectQuery, CommandType.Text, sqlParameters.ToArray()))
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        while (dataReader.Read())
                        {
                            items.Add(GetCampaignsListForViewFromReader(dataReader, userName));
                        }
                    }
                    result.Items = items;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GetCampaignsOutputDto GetFtpDetailsByCompanyIdAndDivisionrId(string Query, List<SqlParameter> sqlParameters)
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = new GetCampaignsOutputDto();

            using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result = new GetCampaignsOutputDto
                        {
                            FTPSite = dataReader["cFTPServer"].ToString(),
                            UserName = dataReader["cUserID"].ToString(),
                            EmailAddress = dataReader["cEmail"].ToString(),
                            FTPPassword = dataReader["cPassword"].ToString()

                        };
                    }

                }
            }
            return result;
        }

        public List<LayoutTemplateDto> GetOutputLayoutTemplate(int buildId, int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var outputlayoutTemplete = new List<LayoutTemplateDto>();
            using (var command = _databaseHelper.CreateCommand($@"
                                                SELECT ID,OrderID,cFieldName,iExportOrder,cCalculation,dCreatedDate,cCreatedBy,dModifiedDate,cModifiedBy,iWidth,cOutputFieldName,cTableNamePrefix,iIsCalculatedField,
                                CASE WHEN cTableNamePrefix ='' THEN cCalculation 
                                ELSE CASE WHEN EXISTS( SELECT cFieldDescription FROM tblBuildTableLayout BTL 
						                                inner join tblBuildTable on tblBuildTable.ID = BTL.BuildTableID
						                                and BTL.cFieldName = tblOrderExportLayout.CFIELDnAME AND  
						                                ( tblBuildTable.cTableName LIKE cTableNamePrefix + '[_]%' OR (tblBuildTable.LK_TableType='M' AND cTableNamePrefix='MainTable' ))AND tblBuildTable.BuildID = {buildId})
                                THEN (SELECT cFieldDescription FROM tblBuildTableLayout BTL 
						                                inner join tblBuildTable on tblBuildTable.ID = BTL.BuildTableID
						                                and BTL.cFieldName = tblOrderExportLayout.CFIELDnAME AND  
						                                ( tblBuildTable.cTableName LIKE cTableNamePrefix + '[_]%' OR (tblBuildTable.LK_TableType='M' AND cTableNamePrefix='MainTable' ))AND tblBuildTable.BuildID = {buildId})
                                else
	                                cOutputFieldName
                                END
		                                  END AS FLDDESCR,
                                     CASE WHEN cTableNamePrefix ='' THEN 1  ELSE 0 END AS IsCalculated
	                                 FROM tblOrderExportLayout WHERE OrderID = {campaignId} ORDER BY iExportOrder", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        outputlayoutTemplete.Add(new LayoutTemplateDto { Order = Convert.ToInt32(dataReader["iExportOrder"]), 
                            FieldName = dataReader["cOutputFieldName"].ToString(), 
                            Formula = ((dataReader["iIsCalculatedField"] is DBNull ? false : Convert.ToBoolean(dataReader["iIsCalculatedField"])) || dataReader["FLDDESCR"].ToString().Trim() == "") ? dataReader["cCalculation"].ToString() : dataReader["FLDDESCR"].ToString(), 
                            Width = dataReader["iWidth"].ToString() });
                    }
                }
            }
            return outputlayoutTemplete;
        }

        public List<DropdownOutputDto> GetCampaignSortValue(int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var outputSortValues = new List<DropdownOutputDto>();
            using (var command = _databaseHelper.CreateCommand($@"
                                                SELECT 0 as iDisplayOrder,'KeyCode1' as cFieldName, 'KeyCode1' as cFieldDescription UNION ALL
                                                Select iDisplayOrder,BTL.cFieldName, BTL.cFieldDescription from tblBuildTableLayout BTL inner join tblBuildTable BT on BT.ID  = BTL.BuildTableID
                                                inner join tblBuild B on B.ID = BT.BuildID inner join tblOrder O on O.BuildID =  B.ID
                                                where BTL.iAllowSorting = 1 And O.ID = {campaignId} order by iDisplayOrder", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        outputSortValues.Add(new DropdownOutputDto { Label = dataReader["cFieldDescription"].ToString(), Value = dataReader["cFieldName"].ToString() });
                    }
                }
            }

            return outputSortValues;
        }

        public List<GetCampaignsOutputDto> GetCampaignShipToValues(string Query, List<SqlParameter> sqlParameters)
        {
            _databaseHelper.EnsureConnectionOpen();
            var outputShipToValues = new List<GetCampaignsOutputDto>();
            //using (var command = _databaseHelper.CreateCommand($@"
            //                                   SELECT ID, CFIRSTNAME +' '+ CLASTNAME as cName , cEmail as cEmailAddress, cCompany, ID as CompID from tblDivisionShipTo  WHERE iIsActive=1 And DivisionID =  {divisionId} Order By cCompany ASC", CommandType.Text))
            using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var name = dataReader["cName"].ToString().Equals(" ") ? string.Empty : "(" + dataReader["cName"] + ")";
                        outputShipToValues.Add(new GetCampaignsOutputDto { Id = Convert.ToInt32(dataReader["CompID"]), Label = dataReader["CompID"] + " : " + dataReader["cCompany"] + " " + name });

                    }
                }
            }

            return outputShipToValues;
        }

        public List<GetCampaignsOutputDto> GetAllOutputLayouts(int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var oultputlayouts = new List<GetCampaignsOutputDto>();
            using (var command = _databaseHelper.CreateCommand($@"
                                             SELECT EL.ID, EL.cDescription, EL.iHasKeyCode
                                         FROM tblExportLayout EL  INNER JOIN tblBuild B on EL.DatabaseID = B.DatabaseID
                                         INNER JOIN  tblOrder O ON O.BuildID = B.ID INNER JOIN tblOffer F on  F.ID = O.OfferID
                                         INNER JOIN  tblMailer M ON O.MailerID  = M.ID 
                                         INNER JOIN tblGroupBroker GB on GB.GroupID = EL.GroupID  and M.BrokerID = GB.BrokerID
                                         WHERE EL.iIsActive = 1   AND ((F.LK_OfferType = 'P' and EL.iHasPhone = 0) 
                                         OR (F.LK_OfferType = 'T'))  And O.ID = {campaignId} Order By cDescription ASC", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        oultputlayouts.Add(new GetCampaignsOutputDto { Id = Convert.ToInt32(dataReader["ID"]), Label = dataReader["ID"] + " : " + dataReader["cDescription"], LayoutDescription = dataReader["cDescription"].ToString(), iHasKeyCode = Convert.ToInt32(dataReader["iHasKeyCode"]) });
                    }
                }
            }

            return oultputlayouts;
        }

        public async Task<GetCampaignsListForView> getOfferMailerBuild(string Query, List<SqlParameter> sqlParameters)
        {
            var result = new GetCampaignsListForView();
            _databaseHelper.EnsureConnectionOpen();

            try
            {
                using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        while (dataReader.Read())
                        {
                            result = new GetCampaignsListForView
                            {
                                MailerId = Convert.ToInt32(dataReader["MailerID"]),
                                Mailer = (dataReader["MailerCompany"]).ToString(),
                                OfferID = Convert.ToInt32(dataReader["OfferID"]),
                                OfferName = (dataReader["OfferName"]).ToString()
                            };
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<int> GetDatabaseIdByUserID(int userID)
        {
            _databaseHelper.EnsureConnectionOpen();
            var databaseIds = new List<int>();
            using (var command = _databaseHelper.CreateCommand(@"
                                                 SELECT A.ID 
                                                 FROM tblDatabase A  WITH (NOLOCK) INNER JOIN tblUserDatabase B  
                                                 WITH (NOLOCK) ON A.ID = B.DatabaseID 
                                                 WHERE B.UserID = " + userID + "Order by A.cDatabaseName ", CommandType.Text))

            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        databaseIds.Add(Convert.ToInt32(dataReader[0]));
                    }
                }
            }

            return databaseIds;
        }

        public int GetDivisionIDFromOrderID(string Query, List<SqlParameter> sqlParameters)
        {
            _databaseHelper.EnsureConnectionOpen();
            var iDivisioID = 0;
            using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
            {
                iDivisioID = Convert.ToInt32(command.ExecuteScalar());
            }
            return iDivisioID;
        }

        public void CopyCampaign(CampaignDto campaign)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@OrderID", campaign.ID));
            sqlParameters.Add(new SqlParameter("@MailerID", campaign.MailerID));
            sqlParameters.Add(new SqlParameter("@OfferID", campaign.OfferID));
            sqlParameters.Add(new SqlParameter("@BuildID", campaign.BuildID));
            sqlParameters.Add(new SqlParameter("@Description", campaign.cDescription));
            sqlParameters.Add(new SqlParameter("@UserID", campaign.UserID));
            sqlParameters.Add(new SqlParameter("@InitiatedBy", campaign.cCreatedBy));

            using (var command = _databaseHelper.CreateCommand("usp_CopyOrder", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }
        }
        public void CopyDivCampaign(CampaignDto campaign)
        {
            _databaseHelper.EnsureConnectionOpen();

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@OrderID", campaign.ID));
            sqlParameters.Add(new SqlParameter("@MailerID", campaign.MailerID));
            sqlParameters.Add(new SqlParameter("@OfferID", campaign.OfferID));
            sqlParameters.Add(new SqlParameter("@BuildID", campaign.BuildID));
            sqlParameters.Add(new SqlParameter("@Description", campaign.cDescription));
            sqlParameters.Add(new SqlParameter("@UserID", campaign.UserID));
            sqlParameters.Add(new SqlParameter("@DivisionMailerID", campaign.DivisionMailerID));
            sqlParameters.Add(new SqlParameter("@DivisionBrokerID", campaign.DivisionBrokerID));
            sqlParameters.Add(new SqlParameter("@OfferName", campaign.cOfferName));
            sqlParameters.Add(new SqlParameter("@InitiatedBy", campaign.cCreatedBy));

            using (var command = _databaseHelper.CreateCommand("usp_CopyOrderDiv", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();

            }

        }

        public List<GetCampaignsListForView> GetTopNCampaigns(string cDescription,string mailer,int numberOfCopies,string userName,int userID,string DatabaseID)
        {            
            _databaseHelper.EnsureConnectionOpen();
            var result = new List<GetCampaignsListForView>();
            var selectBuild = "B.cDescription as BuildDescription";
            var selectDatabase = "D.cDatabaseName as DatabaseName";
            var DBID = $"{ DatabaseID }";
            var selectCustomer = $"CASE WHEN (cBillingCompany IS NOT NULL AND cBillingCompany<>'')  THEN cBillingCompany ELSE(CASE WHEN d.ID = {DBID} THEN M.cCompany ELSE DM.cCompany END) END as CustomerDescription";
            
            var additionalSelect = $"{selectBuild},{selectDatabase},{selectCustomer},";
            var campDescription = $"'{cDescription}'";
            
            using (var command = _databaseHelper.CreateCommand($@"
                    select top {numberOfCopies} O.ID as campaignId,D.DivisionID as DivisionID,B.DatabaseID as DatabaseID,O.BuildID as BuildID,B.cBuild as cBuild,O.cDescription As campaignDescription,
                    {additionalSelect} O.iProvidedCount As providedQty,O.MailerID As MailerID,O.iAvailableQty as availableQty,OS.dCreatedDate as dOrderCreatedDate,OS.iStatus As status,o.cCreatedBy as CreatedBy, O.iSplitType,o.cExportLayout
                    from tblOrder O left join tblMailer M on O.MailerID=M.ID
                    inner join tblOrderStatus OS on O.ID=OS.OrderID
                    inner join tblBuild B on O.BuildID=B.ID
                    
                    left join tblDivisionMailer DM on O.DivisionMailerID=DM.ID
                    inner join tblDatabase D on D.ID = B.DatabaseID
                    WHERE O.cDescription={campDescription} AND 
                    O.MailerID = {mailer} AND O.userID={userID} AND d.ID = {DBID} order by 1 DESC", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new GetCampaignsListForView
                        {
                            OrderCreatedDate = Convert.ToDateTime(dataReader["dOrderCreatedDate"]).ToString("MM/dd/yyyy"),
                            StatusDescription = OrderStatusHelper.GetOrderStatusNameByStatus(Convert.ToInt32(dataReader["status"])),
                            CampaignId = Convert.ToInt32(dataReader["campaignId"]),
                            CampaignDescription = (dataReader["campaignDescription"]).ToString(),
                            ProvidedQty = Convert.ToInt32(dataReader["providedQty"]),
                            iAvailableQty = Convert.ToInt32(dataReader["availableQty"]),
                            BuildID = Convert.ToInt32(dataReader["BuildID"]),
                            DatabaseID = Convert.ToInt32(dataReader["DatabaseID"]),
                            DivisionId = Convert.ToInt32(dataReader["DivisionID"]),
                            SplitType = Convert.ToInt32(dataReader["iSplitType"]),
                            BuildDescription = (dataReader["campaignDescription"]).ToString(),
                            CustomerDescription = (dataReader["CustomerDescription"]).ToString(),
                            DatabaseName = Convert.ToString(dataReader["DatabaseName"]),
                            CreatedBy = Convert.ToString(dataReader["CreatedBy"]),
                            IsLocked = Convert.ToString(dataReader["campaignDescription"]).ToUpper().Contains("LOCKED") && (dataReader["CreatedBy"].ToString().ToLower() != userName.ToLower()), // Yes
                            cExportLayout = dataReader["cExportLayout"].ToString(),
                            MailerId = Convert.ToInt32(dataReader["MailerID"]),
                            Status = Convert.ToInt32(dataReader["status"])
                        });
                    }
                }
            }
            return result;
        }

        public void CopyOrderExportLayout(int iOrderID, int iExportLayoutID, string sInitiatedBy)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@OrderID", iOrderID));
            sqlParameters.Add(new SqlParameter("@ExportLayoutID", iExportLayoutID));
            sqlParameters.Add(new SqlParameter("@InitiatedBy", sInitiatedBy));

            using (var command = _databaseHelper.CreateCommand("usp_CopyOrderExportLayout", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }
        }

        public List<GetCampaignMultidimensionalReportForViewDto> GetMultiColumnFields(string iOrderID, string skippedFields)
        {
            _databaseHelper.EnsureConnectionOpen();
            var multidimensionalFields = new List<GetCampaignMultidimensionalReportForViewDto>();
            using (var command = _databaseHelper.CreateCommand($@" SELECT  0 as iDisplayOrder, 'ListID' as cFieldName, 'List ID' as cFieldDescription UNION ALL
                    SELECT  0 as iDisplayOrder, 'KeyCode1' as cFieldName, 'KeyCode1' as cFieldDescription UNION ALL
                    SELECT iDisplayOrder, SUBSTRING( BT.cTableName,0,CHARINDEX('_',BT.cTableName))+'.' + BTL.cFieldName, BTL.cFieldDescription 
                        FROM tblBuildTableLayout BTL inner join tblBuildTable BT on BT.ID  = BTL.BuildTableID
                        INNER JOIN tblBuild B on B.ID = BT.BuildID inner join tblOrder O on O.BuildID =  B.ID
                    WHERE BTL.iIsSelectable = 1 and BTL.cfieldtype='S' AND O.ID = {iOrderID} ", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;
                if (!string.IsNullOrEmpty(skippedFields))
                    command.CommandText += string.Format(@" AND BTL.cFieldName NOT IN ({0})", skippedFields);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        multidimensionalFields.Add(new GetCampaignMultidimensionalReportForViewDto { cFieldName = dataReader["cFieldName"].ToString(), cFieldDescription = dataReader["cFieldDescription"].ToString() });
                    }
                }
            }

            return multidimensionalFields;
        }

        public List<GetExportLayoutSelectedFieldsDto> GetExportLayoutSelectedFields(int buildId, int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var selectedFields = new List<GetExportLayoutSelectedFieldsDto>();
            using (var command = _databaseHelper.CreateCommand($@" SELECT ID,OrderID,cFieldName,iExportOrder,cCalculation,dCreatedDate,cCreatedBy,dModifiedDate,cModifiedBy,iWidth,
		                                               cOutputFieldName,cTableNamePrefix,
                                                       iIsCalculatedField ,  
		                                               CASE WHEN cTableNamePrefix ='' THEN cCalculation 
                                                       ELSE CASE WHEN EXISTS( SELECT cFieldDescription FROM tblBuildTableLayout BTL 
		                                               inner join tblBuildTable on tblBuildTable.ID = BTL.BuildTableID
		                                               and BTL.cFieldName = tblOrderExportLayout.CFIELDnAME AND  
		                                               ( tblBuildTable.cTableName LIKE cTableNamePrefix + '[_]%' OR (tblBuildTable.LK_TableType='M' AND cTableNamePrefix='MainTable' ))AND tblBuildTable.BuildID ={buildId})
                                                       THEN (SELECT cFieldDescription FROM tblBuildTableLayout BTL 
		                                               inner join tblBuildTable on tblBuildTable.ID = BTL.BuildTableID
		                                               and BTL.cFieldName = tblOrderExportLayout.CFIELDnAME AND  
		                                               ( tblBuildTable.cTableName LIKE cTableNamePrefix + '[_]%' OR (tblBuildTable.LK_TableType='M' AND cTableNamePrefix='MainTable' ))AND tblBuildTable.BuildID = {buildId})
                                                       else
	                                                   cOutputFieldName
                                                       END
		                                               END AS FLDDESCR,
                                                       CASE WHEN cTableNamePrefix ='' THEN 1  ELSE 0 END AS IsCalculated,
			                                           BT.ctabledescription+'('+ BT.cTableName+')' as TableDescription

	                                                   FROM tblOrderExportLayout 
		                                               left outer join 
		                                               (select LEFT(cTableName, CHARINDEX('_',cTableName)-1) as cTableName,ctabledescription from tblbuildtable where buildid={buildId}
                                                       UNION 
						                               SELECT  LEFT(cTableName, CHARINDEX('_',cTableName)-1) AS cTableName,ctabledescription
						                               FROM   tblBuild 
							                           INNER JOIN tblExternalBuildTableDatabase ExDB 
								                       ON ExDB.DatabaseID = tblBuild.DatabaseID AND tblBuild.ID ={buildId}
							                           INNER JOIN  tblBuildTable 
								                       ON  tblBuildTable.ID = ExDB.BuildTableID) BT
		                                               on BT.cTableName = CAse when tblOrderExportLayout.ctablenamePrefix = 'MainTable' then 'tblMain' else tblOrderExportLayout.ctablenamePrefix end

            WHERE OrderID = {campaignId} ORDER BY iExportOrder ", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;


                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        selectedFields.Add(new GetExportLayoutSelectedFieldsDto
                        {
                            ID = Convert.ToInt32(dataReader["ID"]),
                            Order = Convert.ToInt32(dataReader["iExportOrder"]),
                            OutputFieldName = dataReader["cOutputFieldName"].ToString(),
                            Formula = dataReader["cCalculation"].ToString(),
                            Width = Convert.ToInt32(dataReader["iWidth"]),
                            tablePrefix = dataReader["cTableNamePrefix"].ToString(),
                            tableDescription = dataReader["TableDescription"].ToString(),
                            iIsCalculatedField = dataReader["iIsCalculatedField"] is DBNull ? false : Convert.ToBoolean(dataReader["iIsCalculatedField"]),
                            fieldName = dataReader["cFieldName"].ToString(),
                            fieldDescription = dataReader["FLDDESCR"].ToString()
                        });
                    }
                }
            }

            return selectedFields;
        }



        public async Task<List<CampaignQueueDto>> GetAllCampaignQueue(int userId, string userName)
        {
            _databaseHelper.EnsureConnectionOpen();
            var campaignQueueList = new List<CampaignQueueDto>();
            using (var command = _databaseHelper.CreateCommand($@" SELECT o.id, o.cdescription, CASE 
		                                WHEN os1.ID IS NULL
			                                THEN os.dcreateddate
		                                WHEN os.iStatus IN (20, 70, 110)
			                                THEN os.dcreateddate
		                                ELSE os1.dcreateddate
		                                END AS dcreateddate, Usr.cFirstName +' '+ Usr.cLastName as FullName, os.cCreatedBy, lk.cDescription AS STATUS, os.iStatus, os.cModifiedBy, os.dModifiedDate, div.cDivisionName, CASE 
		                                WHEN os.iStopRequested = 0
			                                THEN 'No'
		                                ELSE 'Yes'
		                                END AS [Stop], D.cDatabaseName, D.ID AS DatabaseID
                                FROM tblOrder o WITH(NOLOCK) 
                                INNER JOIN tblOrderStatus os  WITH(NOLOCK) ON o.ID = os.OrderID
                                INNER JOIN tblLookup lk  WITH(NOLOCK) ON os.iStatus = RTRIM(lk.cCode)
	                                AND lk.cLookupValue = 'OrderLoadStatus'
	                                AND os.iStatus IN (20, 30, 70, 80, 110, 120)
	                                AND os.iIsCurrent = 1
                                INNER JOIN tblBuild b  WITH(NOLOCK) ON b.ID = o.BuildID
                                LEFT JOIN tblOrderStatus os1  WITH(NOLOCK) ON os1.OrderID = o.ID
	                                AND OS1.ID IN (
		                                SELECT max(ID)
		                                FROM tblOrderStatus os3 WITH(NOLOCK) 
		                                WHERE os3.OrderID = o.ID
			                                AND os3.iIsCurrent = 0
			                                AND os3.iStatus IN (20, 70, 110)
		                                )
                                INNER JOIN tblUserDatabase UD  WITH(NOLOCK) ON b.DatabaseID = UD.DatabaseID
                                INNER JOIN tblDatabase D  WITH(NOLOCK) ON UD.DatabaseID = D.ID
                                INNER JOIN tblDivision Div  WITH(NOLOCK) ON Div.ID = D.DivisionID 
                                LEFT JOIN tblUser Usr WITH(NOLOCK) ON Usr.cUserID = os.cCreatedBy
                                Where UD.UserID={userId} Order By os1.dCreatedDate ASC ", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;

                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        campaignQueueList.Add(new CampaignQueueDto
                        {
                            id = Convert.ToInt32(dataReader["id"]),
                            cdescription = dataReader["cdescription"].ToString(),
                            cDatabaseName = dataReader["cDatabaseName"].ToString(),
                            DatabaseId = Convert.ToInt32(dataReader["DatabaseID"]),
                            cCreatedBy = dataReader["FullName"].ToString(),
                            dcreateddate = Convert.ToDateTime(dataReader["dcreateddate"]).ToString("MM/dd/yyyy HH:mm:ss"),
                            iStatus = dataReader["STATUS"].ToString(),
                            StatusNumber = Convert.ToInt32(dataReader["iStatus"]),
                            iStopRequested = dataReader["Stop"].ToString(),
                            cDivisionName = dataReader["cDivisionName"].ToString(),
                            cModifiedBy = dataReader["cModifiedBy"].ToString(),
                            dModifiedDate = dataReader["dModifiedDate"].ToString(),
                            IsLocked = Convert.ToString(dataReader["cdescription"]).ToUpper().Contains("LOCKED") ? (Convert.ToString(dataReader["cCreatedBy"]).ToLower() != userName.ToLower() ? true : false) : false
                        });
                    }
                }
            }

            return campaignQueueList;
        }

        public List<int> GetExternalBuildTableIDByOrderID(int orderId)
        {

            _databaseHelper.EnsureConnectionOpen();
            var buildTableIds = new List<int>();
            using (var command = _databaseHelper.CreateCommand($@" SELECT ExDB.BuildTableID
                            FROM   tblOrder INNER JOIN tblBuild ON tblBuild.ID = tblOrder.BuildID
                            INNER JOIN tblExternalBuildTableDatabase ExDB on ExDB.DatabaseID = tblBuild.DatabaseID
                            WHERE tblOrder.ID ={orderId}", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;


                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        buildTableIds.Add(Convert.ToInt32(dataReader["BuildTableID"]));
                    }
                }
            }

            return buildTableIds;
        }

        public List<GetCampaignMultidimensionalReportForViewDto> GetExternalDatabaseFields()
        {
            _databaseHelper.EnsureConnectionOpen();
            var multidimensionalFields = new List<GetCampaignMultidimensionalReportForViewDto>();
            using (var command = _databaseHelper.CreateCommand($@" SELECT  BT.id  as ExBTID, BL.ID,iDataLength,cFieldName,cTableName ,
                        iShowTextBox,
                        iShowListBox,
                        iFileOperations,
                        iShowDefault, 
                        cFieldType,
                        iDisplayOrder,
                        iIsListSpecific, 
                        cFieldDescription
                        FROM tblBuildTable BT INNER JOIN tblBuildTableLayout BL ON BL.BuildTableID = BT.ID 
                        inner join (
                        SELECT DISTINCT BuildTableID from tblExternalBuildTableDatabase) ExtDB on BT.id = ExtDB.BuildTableID
                        WHERE BL.iIsSelectable =1 Order By ExBTID, BL.iDisplayOrder, BL.cFieldDescription ", CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;


                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        multidimensionalFields.Add(new GetCampaignMultidimensionalReportForViewDto { cFieldName = dataReader["cFieldName"].ToString(), cFieldDescription = dataReader["cFieldDescription"].ToString(), cFieldType = dataReader["cFieldType"].ToString(), ExtBuildId = Convert.ToInt32(dataReader["ExBTID"]), cTableName = dataReader["cTableName"].ToString() });
                    }
                }
            }

            return multidimensionalFields;
        }

        public List<CampaignExportPartDto> GetExportPartsSelection(int campaignId, int part)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();
            var exportPartSelectionList = new List<CampaignExportPartDto>();

            sqlParameters.Add(new SqlParameter("@OrderID", campaignId));
            sqlParameters.Add(new SqlParameter("@N", part));


            using (var command = _databaseHelper.CreateCommand("usp_GetNExportParts", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.CommandType = CommandType.StoredProcedure;
                //command.ExecuteNonQuery();
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var campaignExportObject = new CampaignExportPartDto();
                        campaignExportObject.iQuantity = new List<string>();
                        campaignExportObject.iDedupeOrderSpecified = Convert.ToInt32(dataReader["iDedupeOrderSpecified"]);
                        campaignExportObject.SegmentID = Convert.ToInt32(dataReader["SegmentID"]);
                        campaignExportObject.SegmentDescription = dataReader["cDescription"].ToString();
                        campaignExportObject.ProvidedQuantity = Convert.ToInt32(dataReader["iProvidedQty"]);
                        campaignExportObject.OutputQuantity = Convert.ToInt32(dataReader["iOutputQty"]);
                        for (int i = 1; i <= part; i++)
                        {
                            campaignExportObject.iQuantity.Add(dataReader["iQuantity" + i].ToString());
                        }
                        exportPartSelectionList.Add(campaignExportObject);
                    }
                }

            }
            return exportPartSelectionList;
        }
        public bool CheckIfOutputFileExists(int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();

            using (var command = _databaseHelper.CreateCommand($@" IF EXISTS (
		                            SELECT ID
		                            FROM tblOrder WITH (NOLOCK)
		                            WHERE ID <> {campaignId} AND cFileLabel<>''
			                            AND cFileLabel = (
				                            SELECT cFileLabel
				                            FROM tblOrder WITH (NOLOCK)
				                            WHERE ID = {campaignId}
				                            )
		                            )
	                            SELECT 'False'
                            ELSE
	                            SELECT 'True'  ", CommandType.Text))

            {
                return Convert.ToBoolean(command.ExecuteScalar());
            }

        }

        public List<DropdownOutputDto> GetSalesRepDropdownValues(int userId = 0)
        {
            var whereCaluase = userId == 0 ? string.Empty : $"where userid = { userId}";
            _databaseHelper.EnsureConnectionOpen();
            var salesRepValues = new List<DropdownOutputDto>();
            using (var command = _databaseHelper.CreateCommand($@"SELECT cSalesRepID, cSalesRepName 
            +' (' + cDivisionName + ')' As cSalesRep FROM tblSalesRep { whereCaluase } ORDER BY cSalesRepName",
                CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        salesRepValues.Add(new DropdownOutputDto
                        {
                            Value = Convert.ToString(dataReader["cSalesRepID"]),
                            Label = Convert.ToString(dataReader["cSalesRep"])
                        });
                    }
                }
            }
            return salesRepValues;
        }

        public int GetOESSStatusByCampaignID(int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var status = 0;
            using (var command = _databaseHelper.CreateCommand($@"SELECT TOP 1 iStatus FROM tblOrderBillingStatus WHERE iIsCurrent = 1
                        AND OrderID = {campaignId}",
                CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        status = Convert.ToInt32(dataReader["iStatus"]);
                    }
                }
            }

            return status;
        }


        public PagedResultDto<GetShippedReportView> GetAllShippedReportsList(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new PagedResultDto<GetShippedReportView>();


            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                command.CommandTimeout = 10 * 60;
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var modelData = new List<GetShippedReportView>();
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                command.CommandTimeout = 10 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var DatabaseName = dataReader["cDatabaseName"] is DBNull ? string.Empty : dataReader["cDatabaseName"].ToString().Trim();
                        if (!string.IsNullOrEmpty(DatabaseName) && (DatabaseName.ToLower().StartsWith(DatabaseNameConst.Infogroup) || DatabaseName.ToLower().EndsWith(DatabaseNameConst.Database)))
                        {
                            DatabaseName = DatabaseName.Replace(DatabaseNameConst.Database, string.Empty, StringComparison.OrdinalIgnoreCase);
                            DatabaseName = DatabaseName.Replace(DatabaseNameConst.Infogroup, string.Empty, StringComparison.OrdinalIgnoreCase);
                        }
                        modelData.Add(new GetShippedReportView
                        {
                            OrderID = dataReader["ID"] is DBNull ? 0 : Convert.ToInt32(dataReader["ID"]),
                            OrderDescription = dataReader["OrderDescription"] is DBNull ? string.Empty : dataReader["OrderDescription"].ToString(),
                            DatabaseName = DatabaseName,
                            Mailer = dataReader["cCompany"] is DBNull ? string.Empty : dataReader["cCompany"].ToString(),
                            Broker = dataReader["Broker"] is DBNull ? string.Empty : dataReader["Broker"].ToString(),
                            BrokerPONumber = dataReader["cBrokerPONo"] is DBNull ? string.Empty : dataReader["cBrokerPONo"].ToString(),
                            ShippedDate = dataReader["dShipDateShipped"] is DBNull ? string.Empty : Convert.ToDateTime(dataReader["dShipDateShipped"]).ToString("MM/dd/yyyy"),
                            PONumber = dataReader["cLVAOrderNo"] is DBNull ? string.Empty : dataReader["cLVAOrderNo"].ToString(),
                            Type = dataReader["LK_OfferType"] is DBNull ? string.Empty : dataReader["LK_OfferType"].ToString(),
                            ProvidedQuantity = dataReader["iProvidedCount"] is DBNull ? 0 : Convert.ToInt32(dataReader["iProvidedCount"].ToString()),
                            NetOrder = dataReader["NetOrder"] is DBNull ? string.Empty : dataReader["NetOrder"].ToString(),
                            NetUsage = dataReader["NetUsage"] is DBNull ? string.Empty : dataReader["NetUsage"].ToString(),
                            DecoyKey = dataReader["cDecoyKey"] is DBNull ? string.Empty : dataReader["cDecoyKey"].ToString(),
                            CreatedBy = dataReader["cCreatedBy"] is DBNull ? string.Empty : dataReader["cCreatedBy"].ToString(),
                            ShippedBy = dataReader["ShippedBy"] is DBNull ? string.Empty : dataReader["ShippedBy"].ToString(),
                            ExportLayoutName = dataReader["ExportLayoutName"] is DBNull ? string.Empty : dataReader["ExportLayoutName"].ToString().Trim(),
                            OESSInvoiceTotal = dataReader["iOESSInvoiceTotal"] is DBNull ? string.Empty : dataReader["iOESSInvoiceTotal"].ToString(),
                            OESSInvoiceNumber = dataReader["cOESSInvoiceNumber"] is DBNull ? string.Empty : dataReader["cOESSInvoiceNumber"].ToString()
                        });
                    }
                }
                result.Items = modelData;
            }
            return result;
        }

        public BuildDetails GetBuildDetails(int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var build = new BuildDetails();
            using (var command = _databaseHelper.CreateCommand($@"SELECT tblBuild.cBuild , tblBuild.ID ,tblBuild.DatabaseID, D.DivisionID from tblBuild WITH (NOLOCK) INNER JOIN tblDatabase D WITH (NOLOCK) on tblBuild.DatabaseID= D.ID
                             INNER JOIN tblOrder WITH (NOLOCK) on tblBuild.ID = tblOrder.BuildID Where tblOrder.ID =  {campaignId}",
                CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        build.Id = Convert.ToInt32(dataReader["ID"]);
                        build.BuildID = Convert.ToInt32(dataReader["cBuild"]);
                        build.DatabaseID = Convert.ToInt32(dataReader["DatabaseID"]);
                        build.DivisionID = Convert.ToInt32(dataReader["DivisionID"]);
                    }
                }
            }

            return build;
        }
        public async Task<GetCampaignsListForView> GetCampaignById(Tuple<string, List<SqlParameter>> query, string loginUsername)
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = new GetCampaignsListForView();
            try
            {
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        while (dataReader.Read())
                        {
                            return GetCampaignsListForViewFromReader(dataReader, loginUsername);
                        }
                    }
                }
                return result;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DropdownOutputDto> GetCampaignStatusById(Tuple<string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = new DropdownOutputDto();
            try
            {
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        while (dataReader.Read())
                        {
                            var status = Convert.ToInt32(dataReader["iStatus"]);
                            return new DropdownOutputDto
                            {
                                Value = status,
                                Label = OrderStatusHelper.GetOrderStatusNameByStatus(status)
                            };
                        }
                    }
                }
                return result;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        private GetCampaignsListForView GetCampaignsListForViewFromReader(DbDataReader dataReader, string loginUsername)
        {
            var campaignDto = new GetCampaignsListForView
            {
                OrderCreatedDate = Convert.ToDateTime(dataReader["dOrderCreatedDate"]).ToString("MM/dd/yyyy"),
                Status = Convert.ToInt32(dataReader["status"]),
                StatusDescription = OrderStatusHelper.GetOrderStatusNameByStatus(Convert.ToInt32(dataReader["status"])),
                CampaignId = Convert.ToInt32(dataReader["campaignId"]),
                CampaignDescription = (dataReader["campaignDescription"]).ToString(),
                ProvidedQty = Convert.ToInt32(dataReader["providedQty"]),
                iAvailableQty = Convert.ToInt32(dataReader["availableQty"]),
                BuildID = Convert.ToInt32(dataReader["BuildID"]),
                DatabaseID = Convert.ToInt32(dataReader["DatabaseID"]),
                DivisionId = Convert.ToInt32(dataReader["DivisionID"]),
                SplitType = Convert.ToInt32(dataReader["iSplitType"]),
                BuildDescription = (dataReader["BuildDescription"]).ToString(),
                CustomerDescription = (dataReader["CustomerDescription"]).ToString(),
                DatabaseName = Convert.ToString(dataReader["DatabaseName"]),
                CreatedBy = Convert.ToString(dataReader["CreatedBy"]),
                IsLocked = Convert.ToString(dataReader["campaignDescription"]).ToUpper().Contains("LOCKED") && (dataReader["CreatedBy"].ToString().ToLower() != loginUsername.ToLower()), // Yes
                cExportLayout = dataReader["cExportLayout"].ToString(),
                MailerId = Convert.ToInt32(dataReader["MailerID"])
            };
            int.TryParse(dataReader["cBuild"].ToString(), out int build);
            campaignDto.Build = build;

            var columnExists = Enumerable.Range(0, dataReader.FieldCount).Any(i => string.Equals(dataReader.GetName(i), "cLVAOrderNo", StringComparison.OrdinalIgnoreCase));
            if (columnExists)
            {
                campaignDto.PoOrderNumber = Convert.ToString(dataReader["cLVAOrderNo"]);
            }

            columnExists = Enumerable.Range(0, dataReader.FieldCount).Any(i => string.Equals(dataReader.GetName(i), "SegmentID", StringComparison.OrdinalIgnoreCase));
            if (columnExists && dataReader["SegmentID"] != DBNull.Value)
            {
                campaignDto.SegmentID = Convert.ToInt32(dataReader["SegmentID"]);
            }

            if (!string.IsNullOrEmpty(campaignDto.DatabaseName) && (campaignDto.DatabaseName.ToLower().StartsWith(DatabaseNameConst.Infogroup) || campaignDto.DatabaseName.ToLower().EndsWith(DatabaseNameConst.Database)))
            {
                campaignDto.DatabaseName = campaignDto.DatabaseName.Replace(DatabaseNameConst.Database, "", StringComparison.OrdinalIgnoreCase);
                campaignDto.DatabaseName = campaignDto.DatabaseName.Replace(DatabaseNameConst.Infogroup, "", StringComparison.OrdinalIgnoreCase);
            }
            return campaignDto;
        }

        public PagedResultDto<GetSelectionFieldCountReportView> GetAllSelectionFieldCountReportsList(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = new PagedResultDto<GetSelectionFieldCountReportView>();
            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))            
            {
                command.CommandTimeout = 10 * 30;
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());                                              
                command.Parameters.Clear();
               
            }
            var modelData = new List<GetSelectionFieldCountReportView>();
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                command.CommandTimeout = 10 * 30;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {                        
                        modelData.Add(new GetSelectionFieldCountReportView
                        {
                                cQuestionFieldName = dataReader["cQuestionFieldName"] is DBNull ? string.Empty: dataReader["cQuestionFieldName"].ToString(),
                                cDescription = dataReader["cDescription"] is DBNull ? string.Empty : dataReader["cDescription"].ToString(),                              
                                count = Convert.ToInt32(dataReader["Count"]),
                                iStatus=Convert.ToInt32(dataReader["iStatus"])
                        });                       
                    }                                      
                }
                result.Items = modelData;
                
               
            }
            return result;
        }

         public PagedResultDto<GetOrderDetailsView> GetOrderDetailsList(Tuple<string, string, List<SqlParameter>> query)        
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = new PagedResultDto<GetOrderDetailsView>();
            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                command.CommandTimeout = 10 * 30;
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var modelData = new List<GetOrderDetailsView>();            
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                command.CommandTimeout = 10 * 30;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        modelData.Add(new GetOrderDetailsView
                        {
                            OrderId = Convert.ToInt32(dataReader["ID"]),
                            cDescription = dataReader["cDescription"] is DBNull ? string.Empty : dataReader["cDescription"].ToString(),
                            iProvidedCount = Convert.ToInt32(dataReader["iProvidedCount"]),
                            dCreatedDate = dataReader["dCreatedDate"] is DBNull ? string.Empty : dataReader["dCreatedDate"].ToString(),
                            CreatedBy = dataReader["CreatedBy"] is DBNull ? string.Empty : dataReader["CreatedBy"].ToString()
                        });                        
                    }
                }
                result.Items = modelData;
               
            }
            return result;
        }

        public async Task<PagedResultDto<GetCampaignsListForView>> GetAllFastCountCampaignsList(string selectQuery, string countQuery, List<SqlParameter> sqlParameters,string username)
        {
            _databaseHelper.EnsureConnectionOpen();
            try
            {
                var result = new PagedResultDto<GetCampaignsListForView>();

                using (var command = _databaseHelper.CreateCommand(countQuery, CommandType.Text, sqlParameters.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var items = new List<GetCampaignsListForView>();
                using (var command = _databaseHelper.CreateCommand(selectQuery, CommandType.Text, sqlParameters.ToArray()))
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        while (dataReader.Read())
                        {
                            var fcData = GetCampaignsListForViewFromReader(dataReader, username);
                            if (fcData.SegmentID > 0)
                                items.Add(fcData);
                        }
                    }
                    result.Items = items;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
