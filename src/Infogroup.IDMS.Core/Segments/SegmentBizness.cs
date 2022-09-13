using Abp.Domain.Services;
using Abp.UI;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Segments.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.Segments
{
    public class SegmentBizness : DomainService
    {
       
        public Tuple<string, string, List<SqlParameter>> GetAllSegmentsList(GetSegmentListInput input)
        {
            try
            {
                var query = new QueryBuilder();
                query.AddSelect($"S.ID,S.OrderID,S.iDedupeOrderSpecified,S.cDescription,S.cKeyCode1,S.cKeyCode2,S.iRequiredQty,S.iProvidedQty,S.iOutputQty,S.iAvailableQty,S.cMaxPerGroup, S.iGroup");
                query.AddFrom("tblSegment", "S");
                query.AddWhere("", "iIsOrderLevel", "EQUALTO", "0");
                query.AddWhere("AND", "S.OrderID", "EQUALTO", input.OrderId.ToString());
                query.AddSort(input.Sorting ?? "iDedupeOrderSpecified asc");
                query.AddOffset($"OFFSET {input.SkipCount} ROWS FETCH NEXT {input.MaxResultCount} ROWS ONLY;");
                // Query to bind the grid
                (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
                // Query to get the total count of records
                var sqlCount = query.BuildCount().Item1;
                return new Tuple<string, string, List<SqlParameter>>(sqlSelect, sqlCount, sqlParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Tuple<string, List<SqlParameter>> GetSegmentForViewByIdQuery(int Id)
        {
            try
            {
                var query = new QueryBuilder();
                query.AddSelect($"TOP 1 S.ID,S.OrderID,S.iDedupeOrderSpecified,S.cDescription,S.cKeyCode1,S.cKeyCode2,S.iRequiredQty,S.iProvidedQty,S.iOutputQty,S.iAvailableQty,S.cMaxPerGroup, S.iGroup");
                query.AddFrom("tblSegment", "S");
                query.AddWhere("", "S.ID", "EQUALTO", Id.ToString());
                (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
                return new Tuple<string, List<SqlParameter>>(sqlSelect, sqlParams);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }




    }
}
