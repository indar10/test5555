using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Infogroup.IDMS.SegmentLists.Dtos;
using Infogroup.IDMS.Common;
using System.Linq;

namespace Infogroup.IDMS.SegmentLists
{
    public class SegmentListBizness : DomainService
    {

        public Tuple<string, List<SqlParameter>> GetApprovedSources(GetAllApprovedSourcesInput filters)
        {
            try
            {
                var unsavedLists = filters.UnsavedListIDs.Select(i => i.ToString()).ToArray();
                if(!string.IsNullOrEmpty(filters.Filter))
                {
                    filters.Filter = filters.Filter.Trim();
                }
                var isListIDs = Validation.ValidationHelper.IsNumeric(filters.Filter);
                var query = new QueryBuilder();
                query.AddSelect("MasterLoL.ID,MasterLoL.cListName");
                query.AddFrom("tblOrderCASApproval", "OrderCASApproval");
                query.AddNoLock();
                query.AddJoin("tblMasterLoL", "MasterLoL", "MasterLOLID", "OrderCASApproval", "INNER JOIN", "ID"); 
                query.AddNoLock();
                query.AddJoin("tblSegment", "Segment", "OrderID", "OrderCASApproval", "INNER JOIN", "OrderID");
                query.AddNoLock();
                query.AddWhere("AND", "OrderCASApproval.cStatus", "EQUALTO", "A");
                if (filters.UnsavedListIDs.Count > 0)
                {
                    query.AddWhere("AND", "MasterLoL.ID", "NOT IN", unsavedLists);
                }
                if (isListIDs)
                {
                    var IdsToFilter = filters.Filter.Split(',').Select(id => id.Trim()).ToArray();
                    query.AddWhere("AND", "MasterLoL.ID", "IN", IdsToFilter);
                }
                else
                    query.AddWhere("AND", "MasterLoL.cListName", "LIKE", filters.Filter);
                query.AddWhere("AND", "Segment.ID", "EQUALTO", filters.SegmentID.ToString());
                query.AddSort("ID ASC");
                (string sql, List<SqlParameter> sqlParams) = query.Build();
                return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

