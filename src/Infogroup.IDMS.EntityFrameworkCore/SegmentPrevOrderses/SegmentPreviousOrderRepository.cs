using Abp.Data;
using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.SegmentPrevOrderses;
using Infogroup.IDMS.SegmentPrevOrderses.Dtos;
using Infogroup.IDMS.SegmentSelections.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Infogroup.IDMS.SegmentPrevOrder
{
    public class SegmentPreviousOrderRepository : IDMSRepositoryBase<SegmentPrevOrders, int>, ISegmentPreviousOrderRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        public SegmentPreviousOrderRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper, IRedisIDMSConfigurationCache idmsConfigurationCache) : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
            _idmsConfigurationCache = idmsConfigurationCache;
        }

        // iDatabaseId could be DivisionalId if it is a divisional Database
        public async Task<List<GetSegmentPrevOrdersForViewDto>> GetAllPreviousOrders(int iDatabaseID, GetPreviousOrdersFilters filters, int userID, string shortWhere, bool isDivisional, string defaultMatchLevel)
        {
            _databaseHelper.EnsureConnectionOpen();
            var isOrderId = Validation.ValidationHelper.IsNumeric(filters.filter);
            var orderIdFilter = string.Empty;
            var descriptionFilter = string.Empty;
            var shortSearch = string.Empty;
            if (!string.IsNullOrEmpty(filters.filter))
            {
                filters.filter = filters.filter.Trim();
            }
            if (!string.IsNullOrEmpty(shortWhere))
            {
                shortSearch = $"AND ({shortWhere})";
            }
            else
            {
                if (isOrderId)
                    orderIdFilter = $"And O.ID IN({filters.filter})";
                else
                    descriptionFilter = $"And O.cDescription like @Filter";
            }

            var sDBCondition = " ";

            if (!isDivisional) {
                //AllowPreviousOrderAcrossDB values will allow all DBs to be added as previous order
                string allowedAcrossDBList = _idmsConfigurationCache.GetConfigurationValue("AllowPreviousOrderAcrossDB", 0).cValue;
                if (allowedAcrossDBList != "" || allowedAcrossDBList != null) {
                    List<string> allDBArray = allowedAcrossDBList.Split(",").ToList();
                    if (!allDBArray.Contains(Convert.ToString(iDatabaseID))) {
                        sDBCondition = $" AND M.DatabaseID = {iDatabaseID} ";
                    }
                }
                else {
                    sDBCondition = $" AND M.DatabaseID = {iDatabaseID} ";
                }
            }
            else {
                sDBCondition =   $" AND D.DivisionID = {iDatabaseID}";
            }

            var query = $@"SELECT  DISTINCT TOP 100 (Select ID from tblSegmentPrevOrders WHERE  SegmentID = {filters.SegmentID} and PrevOrderID=O.ID) as POId,
                        (Select cIncludeExclude from tblSegmentPrevOrders WHERE  SegmentID = {filters.SegmentID} and PrevOrderID=O.ID) as cIncludeExclude,
						(Select cCompareFieldName from tblSegmentPrevOrders WHERE  SegmentID = {filters.SegmentID} and PrevOrderID=O.ID) as cCompareFieldName,
                        (Select cMatchFieldName from tblSegmentPrevOrders WHERE   SegmentID = {filters.SegmentID} and PrevOrderID=O.ID) as cMatchFieldName,
                        O.ID as OrderID,O.cDescription, O.cLVAOrderNo,
                        tblOrderStatus.dCreatedDate  
                        FROM tblorder O {sNoLock}
                        INNER JOIN tblOrderStatus WITH (NOLOCK) ON tblOrderStatus.OrderID = O.ID 
                        AND tblOrderStatus.iIsCurrent = 1 
                        AND tblOrderStatus.dCreatedDate >= dateadd(day,datediff(day,0,getdate())-400,0) 
                        AND tblOrderStatus.iStatus > = 40 AND tblOrderStatus.iStatus NOT IN (150,100,50)
                        INNER JOIN  tblMailer M {sNoLock} ON O.MailerID = M.ID 
                        INNER JOIN tblDatabase D  {sNoLock} ON D.ID = M.DatabaseID
                        INNER JOIN tblOffer F {sNoLock} ON O.OfferID = F.ID
                        INNER JOIN tblGroupBroker GB {sNoLock} ON M.BrokerID=GB.BrokerID
                        INNER JOIN tblUserGroup UG {sNoLock} ON UG.GroupID = GB.GroupID 
                        LEFT JOIN  tblDivisionMailer DM {sNoLock} ON O.DivisionMailerID = DM.ID 
		                    AND not exists (Select  ID from tblSegmentPrevOrders WHERE tblSegmentPrevOrders.PrevOrderID = O.ID  and  SegmentID = {filters.SegmentID} )    
	                    INNER JOIN tblSegment on tblSegment.OrderID <> O.id and tblSegment.ID =  {filters.SegmentID}    
                        WHERE UG.UserID= {userID} {sDBCondition} {descriptionFilter} {orderIdFilter} {shortSearch}
                        Order By tblOrderStatus.dCreatedDate DESC ";

            var result = new List<GetSegmentPrevOrdersForViewDto>();

            using (var command = _databaseHelper.CreateCommand(query, CommandType.Text))
            {
                command.Parameters.Add(new SqlParameter("@Filter", $"%{filters.filter}%"));
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new GetSegmentPrevOrdersForViewDto
                        {
                            PreviousOrderID = dataReader["POId"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(dataReader["POId"]),
                            OrderID = Convert.ToInt32(dataReader["OrderID"]),
                            Description = (dataReader["cDescription"]).ToString(),
                            cIncludeOrExclude = dataReader["cIncludeExclude"].Equals(DBNull.Value) ? "Exclude" : dataReader["cIncludeExclude"].ToString().Equals("I") ? "Include" : "Exclude",
                            cIndividualOrCompany = string.IsNullOrEmpty(defaultMatchLevel) || defaultMatchLevel == "I" ? "Individual" : "Company",
                            action = ActionType.None,
                            cLVAOrderNo = dataReader["cLVAOrderNo"].ToString()
                        });
                    }
                }
            }
            return result;
        }

        public string ValidateKeyColumnWithPrevOrders(string OrderID, string PreviousOrderID)
        {
            _databaseHelper.EnsureConnectionOpen();
            var query = $"SELECT TOP 1 A.cFieldDescription FROM tblBuildTableLayout A INNER JOIN tblBuildTable B on B.ID=A.BuildTableID  and B.LK_TableType='M' INNER JOIN tblOrder O  on B.BuildID = O.BuildID INNER JOIN tblBuildTableLayout C  on A.cFieldName<>C.cFieldName OR A.cDataType <> C.cDataType OR A.iDataLength <> C.iDataLength  INNER JOIN tblBuildTable D on D.ID=C.BuildTableID and D.LK_TableType='M' INNER JOIN tblOrder PO  on D.BuildID = PO.BuildID WHERE O.ID = { OrderID } AND PO.ID = { PreviousOrderID } AND A.iKeyCOlumn = 1 AND C.iKeyCOlumn = 1 ";
            var result = string.Empty;

            using (var command = _databaseHelper.CreateCommand(query, CommandType.Text))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result = dataReader["cFieldDescription"].ToString();
                    }
                }
            }
            return result;
        }

        public List<string> GetValidPreviousCampaigns(Tuple<string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new List<string>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                        while (dataReader.Read())
                            result.Add(dataReader["ID"].ToString());
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public string BulkOperationOnCampaignHistory(SaveGlobalChangesInputDto input, string initiatedBy)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderID", input.CampaignId),
                new SqlParameter("@cIncludeExclude", input.IncludeExclude),
                new SqlParameter("@cCompareFieldName", input.CompareFieldName),
                new SqlParameter("@CommaSeparatedNumbers", DBNull.Value),
                new SqlParameter("@Option", input.Option),
                new SqlParameter("@Content", input.SearchValue),
                new SqlParameter("@InitiatedBy", initiatedBy),
                new SqlParameter("@CommaSeparatedIds", input.TargetSegments)
            };

            using (var command = _databaseHelper.CreateCommand("usp_AddPrevOrdersToMultipleSegments", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                var result =  command.ExecuteScalar();
                return result.ToString();
            }

        }

    }
}