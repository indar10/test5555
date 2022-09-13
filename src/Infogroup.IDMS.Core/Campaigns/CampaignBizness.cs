using Abp.Domain.Services;
using Infogroup.IDMS.Campaigns.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Campaigns
{
    public class CampaignBizness : DomainService
    {
        private const string ExactDataBrokerID = "1500";
        public Tuple<string, string, List<SqlParameter>> GetAllCampaignsQuery(GetCampaignListFilters filters, List<int> DatabaseIds, string DivisionalDatabaseIDs, string shortWhere)
        {
            if (shortWhere != null & shortWhere.Length > 0)
                filters.Filter = "";

            if (!string.IsNullOrEmpty(filters.Filter))
                filters.Filter = filters.Filter.Trim();

            // reges to check if global search value is numeric or alphanumeric
            bool isOrderId = Validation.ValidationHelper.IsNumeric(filters.Filter);
            // to get Database Id's in string Array
            string[] DbIds = DatabaseIds.Select(x => x.ToString()).ToArray();
            // to split comma sepated CampaignId String into string Array
            string[] filtersarray = null;
            var selectBuild = "B.cDescription as BuildDescription";
            var selectDatabase = "D.cDatabaseName as DatabaseName";
            var selectCustomer = $"CASE WHEN (cBillingCompany IS NOT NULL AND cBillingCompany<>'')  THEN cBillingCompany ELSE(CASE WHEN d.ID IN( {DivisionalDatabaseIDs} ) THEN M.cCompany ELSE DM.cCompany END) END as CustomerDescription";
            var additionalSelect = $"{selectBuild},{selectDatabase},{selectCustomer},";

            if (!string.IsNullOrEmpty(filters.Filter))
                filtersarray = filters.Filter.Split(',');
            var query = new Common.QueryBuilder();
            query.AddSelect($"O.ID as campaignId,D.DivisionID as DivisionID,B.DatabaseID as DatabaseID,O.BuildID as BuildID,B.cBuild as cBuild,O.cDescription As campaignDescription,{additionalSelect} O.iProvidedCount As providedQty,O.MailerID As MailerID,O.iAvailableQty as availableQty,OS.dCreatedDate as dOrderCreatedDate,OS.iStatus As status,o.cCreatedBy as CreatedBy, O.iSplitType,o.cExportLayout");
            query.AddFrom("tblOrder", "O");
            query.AddJoin("tblMailer", "M", "MailerID", "O", "LEFT JOIN", "ID");
            query.AddJoin("tblGroupBroker", "GB", "BrokerID", "M");
            query.AddJoin("tblUserGroup", "UG", "GroupID", "GB");
            query.AddJoin("tblOrderStatus", "OS", "ID", "O", "INNER JOIN", "OrderID");
            query.AddJoin("tblBuild", "B", "BuildID", "O", "INNER JOIN", "ID");
            query.AddJoin("tblDatabase", "D", "DatabaseID", "B", "INNER JOIN", "ID");
            query.AddJoin("tblDivisionMailer", "DM", "DivisionMailerID", "O", "LEFT JOIN", "ID");

            query.AddWhere("AND", "B.DatabaseID", "IN", DbIds);
            query.AddWhere("AND", "OS.iIsCurrent", "EQUALTO", "1");
            if (isOrderId)
            {
                query.AddWhere("AND", "O.ID", "IN", filtersarray);
            }
            else
            {
                query.AddWhere("AND", "O.cDescription", "LIKE", filters.Filter);
            }

            query.AddWhere("AND", "O.ID", "IN", filters.ID);
            if (filters.selectedDateRange != null)
                query.AddWhere("AND", "OS.dCreatedDate", "BETWEEN", new string[] { filters.selectedDateRange[0].ToString(), filters.selectedDateRange[1].ToString() });

            query.AddWhere("AND", "O.cDescription", "LIKE", filters.Description);
            query.AddWhere("AND", "DM.cCompany", "LIKE", filters.CustomerName);
            query.AddWhere("AND", "B.cDescription", "LIKE", filters.BuildDescription);
            query.AddWhere("AND", "D.cDatabaseName", "LIKE", filters.DatabaseName);
            query.AddWhere("AND", "OS.iStatus", "EQUALTO", filters.Status);
            query.AddWhere("AND", "UG.UserID", "EQUALTO", filters.UserID.ToString());
            query.AddWhere("AND", "O.cCreatedBy", "EQUALTO", filters.UserName);

            if (shortWhere.Length > 0)
                query.AddWhereString($"AND ({shortWhere})");

            query.AddSort(filters.Sorting ?? "OS.dCreatedDate DESC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            query.AddDistinct();

            // Query to bind the grid
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            // Query to get the total count of records
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);

        }


        public Tuple<string, List<SqlParameter>> getOfferByMailerQuery(int mailerID)
        {
            try
            {
                var query = new Common.QueryBuilder();
                query.AddSelect("TOP 1 M.ID as MailerID,M.cCompany as MailerCompany,O.ID as OfferID,O.cOfferName as OfferName");
                query.AddFrom("tblOffer", "O");
                query.AddNoLock();
                query.AddJoin("tblMailer", "M", "MailerID", "O", "INNER JOIN", "ID");
                query.AddWhere("AND", "O.iIsActive", "IN", "1");
                query.AddWhere("AND", "M.ID", "EQUALTO", mailerID.ToString());
                (string sql, List<SqlParameter> sqlParams) = query.Build();
                return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Tuple<string, List<SqlParameter>> GetDivisionIDFromOrderIDQuery(int campaignID)
        {
            try
            {
                var query = new Common.QueryBuilder();
                query.AddSelect("TD.ID as DivisionID");
                query.AddFrom("tblOrder", "O");
                query.AddNoLock();
                query.AddJoin("tblBuild", "bi", "BuildID", "O", "INNER JOIN", "ID");
                query.AddJoin("tblDatabase", "D", "DatabaseID", "bi", "INNER JOIN", "ID");
                query.AddJoin("TBLDIVISION", "TD", "DIVISIONID", "D", "INNER JOIN", "ID");
                query.AddWhere("AND", "O.ID", "IN", campaignID.ToString());

                (string sql, List<SqlParameter> sqlParams) = query.Build();
                return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Tuple<string, List<SqlParameter>> GetFtpDetailsByCompanyIdAndDivisionrId(int companyId, int divisionId)
        {
            try
            {
                var query = new Common.QueryBuilder();
                query.AddSelect("cFTPServer, cUserID , cEmail,cPassword");
                query.AddFrom("tblDivisionShipTo", "");
                query.AddWhere("", "iIsActive", "EQUALTO", "1");
                query.AddWhere("AND", "DivisionID", "EQUALTO", divisionId.ToString());
                query.AddWhere("AND", "ID", "EQUALTO", companyId.ToString());


                (string sql, List<SqlParameter> sqlParams) = query.Build();
                return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Tuple<string, List<SqlParameter>> GetCampaignShipToValues(int divisionId)
        {
            try
            {
                var query = new Common.QueryBuilder();
                query.AddSelect("ID, CFIRSTNAME +' '+ CLASTNAME as cName , cEmail as cEmailAddress, cCompany, ID as CompID");
                query.AddFrom("tblDivisionShipTo", "");
                query.AddWhere("", "iIsActive", "EQUALTO", "1");
                query.AddWhere("AND", "DivisionID", "EQUALTO", divisionId.ToString());
                query.AddSort("cCompany ASC");


                (string sql, List<SqlParameter> sqlParams) = query.Build();
                return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Tuple<string, List<SqlParameter>> GetCampaignByIdQuery(int campaignId, string divisionalDatabaseIds)
        {
            var elseStatement= "ELSE DM.cCompany";
            if(!string.IsNullOrEmpty(divisionalDatabaseIds))
            elseStatement = $@"ELSE ( CASE WHEN d.ID IN({divisionalDatabaseIds}) 
                                           THEN M.cCompany
                                       ELSE DM.cCompany END)";
            
            var query = new Common.QueryBuilder();
            query.AddSelect($@" TOP 1 O.ID as campaignId, 
                                      LTRIM(RTRIM(O.cDescription)) As campaignDescription,
                                      O.iProvidedCount As providedQty,
                                      O.iAvailableQty as availableQty,
                                      O.BuildID as BuildID,
                                      O.MailerID As MailerID,
                                      LTRIM(RTRIM(B.cDescription)) as BuildDescription,
                                      B.DatabaseID as DatabaseID,
                                      LTRIM(RTRIM(D.cDatabaseName)) as DatabaseName,
                                      D.DivisionID as DivisionID,
                                      B.cBuild as cBuild,
                                       CASE WHEN (O.cBillingCompany IS NOT NULL  AND cBillingCompany<>'') 
                                           THEN cBillingCompany 
                                      {elseStatement} 
                                      END as CustomerDescription,
                                      OS.dCreatedDate as dOrderCreatedDate,
                                      OS.iStatus As status,
                                      O.cCreatedBy as CreatedBy,
                                      O.iSplitType,
                                      O.cExportLayout");
            query.AddFrom("tblOrder", "O");
            query.AddJoin("tblMailer", "M", "MailerID", "O", "LEFT JOIN", "ID");
            query.AddJoin("tblOrderStatus", "OS", "ID", "O", "INNER JOIN", "OrderID");
            query.AddJoin("tblBuild", "B", "BuildID", "O", "INNER JOIN", "ID");
            query.AddJoin("tblDatabase", "D", "DatabaseID", "B", "INNER JOIN", "ID");
            query.AddJoin("tblDivisionMailer", "DM", "DivisionMailerID", "O", "LEFT JOIN", "ID");
            query.AddWhere("AND", "OS.iIsCurrent", "EQUALTO", "1");
            query.AddWhere("AND", "O.ID", "EQUALTO", campaignId.ToString());
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sqlSelect.ToString(), sqlParams);

        }

        public Tuple<string, List<SqlParameter>> GetCampaignStatusByIdQuery(int campaignId)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect(" TOP 1 OS.iStatus");
            query.AddFrom("tblOrderStatus", "OS");
            query.AddWhere("AND", "OS.OrderID", "EQUALTO", campaignId.ToString());
            query.AddWhere("AND", "OS.iIsCurrent", "EQUALTO", "1");
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sqlSelect.ToString(), sqlParams);
        }

        public Tuple<string, string, List<SqlParameter>> GetAllFastCountCampaignsQuery(GetFastCountCampaignListFilters filters,string DivisionalDatabaseIDs)
        {
            // check if global search value is numeric or alphanumeric
            bool isOrderId = Validation.ValidationHelper.IsNumeric(filters.OrderId);
            var selectBuild = "B.cDescription as BuildDescription";
            var selectDatabase = "D.cDatabaseName as DatabaseName";
            var selectCustomer = $"CASE WHEN (cBillingCompany IS NOT NULL AND cBillingCompany<>'')  THEN cBillingCompany ELSE(CASE WHEN d.ID IN( {DivisionalDatabaseIDs} ) THEN M.cCompany ELSE DM.cCompany END) END as CustomerDescription";
            var additionalSelect = $"{selectBuild},{selectDatabase},{selectCustomer},";
           
            var query = new Common.QueryBuilder();
            query.AddSelect($"O.ID as campaignId,D.DivisionID as DivisionID,B.DatabaseID as DatabaseID,O.BuildID as BuildID,B.cBuild as cBuild,O.cDescription As campaignDescription,{additionalSelect} O.MailerID As MailerID,OS.dCreatedDate as dOrderCreatedDate,OS.iStatus As status,o.cCreatedBy as CreatedBy, O.iSplitType,o.cExportLayout, O.cLVAOrderNo, ts.id as SegmentID, ts.iProvidedQty as providedQty, ts.iAvailableQty as availableQty");
            query.AddFrom("tblOrder", "O");
            query.AddJoin("tblMailer", "M", "MailerID", "O", "LEFT JOIN", "ID");
            query.AddJoin("tblGroupBroker", "GB", "BrokerID", "M");
            query.AddJoin("tblUserGroup", "UG", "GroupID", "GB");
            query.AddJoin("tblOrderStatus", "OS", "ID", "O", "INNER JOIN", "OrderID");
            query.AddJoin("tblBuild", "B", "BuildID", "O", "INNER JOIN", "ID");
            query.AddJoin("tblDatabase", "D", "DatabaseID", "B", "INNER JOIN", "ID");
            query.AddJoin("tblDivisionMailer", "DM", "DivisionMailerID", "O", "LEFT JOIN", "ID");
            query.AddJoin("AbpUserAccounts", "U", "cCreatedBy", "O", "INNER JOIN", "username");
            query.AddJoin("tblSegment", "ts", "id", "O", "INNER JOIN", "orderID");

            query.AddWhere("AND", "O.DivisionBrokerID", "EQUALTO", ExactDataBrokerID);
            query.AddWhere("AND", "OS.iIsCurrent", "EQUALTO", "1");
            query.AddWhere("AND", "UPPER(O.cNotes)", "EQUALTO", "FASTCOUNT 3.0");
            query.AddWhere("AND", "ts.iIsOrderLevel", "EQUALTO", "0");

            if (filters.DatabaseId != null)
                query.AddWhere("AND", "B.DatabaseID", "IN", filters.DatabaseId.ToArray()); 

            if (isOrderId)
                query.AddWhere("AND", "O.ID", "LIKE", filters.OrderId);

            if (filters.selectedDateRange != null)
                query.AddWhere("AND", "OS.dCreatedDate", "BETWEEN", new string[] { filters.selectedDateRange[0].ToString(), filters.selectedDateRange[1].ToString() });
           
            if(filters.Status!=null)
                query.AddWhere("AND", "OS.iStatus", "IN", filters.Status.ToArray());
           
            if(filters.UserId != null)
                query.AddWhere("AND", "U.USERID", "IN", filters.UserId.ToArray());    
            
            
            query.AddSort(filters.Sorting ?? "OS.dCreatedDate DESC");

            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");

            query.AddDistinct();

            // Query to bind the grid
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            // Query to get the total count of records
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);

        }

    }
}

