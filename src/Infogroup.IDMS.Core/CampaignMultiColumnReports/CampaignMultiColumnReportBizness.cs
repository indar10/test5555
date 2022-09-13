using Abp.Application.Services.Dto;
using Abp.Domain.Services;
using Infogroup.IDMS.Campaigns.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Campaigns
{
    public class CampaignMultiColumnReportBizness : DomainService
    {
        

        public Tuple<string, List<SqlParameter>> GetAllCampaignMultiDimensionalReports(int campaignId)
        {
            try
            {
                var query = new Common.QueryBuilder();
                query.AddSelect("MCol.ID, MCol.cType, case when cType='G' then 'Gross' else 'Net' end as cTypeName, MCol.cFieldsDescription as cDesc, MCol.cSegmentNumbers ,MCol.cFields as cFields,MCol.iMultiColBySegment, case  when MCol.iMultiColBySegment = 1  then 'Segment' else 'Campaign' end as IsMCol");
                query.AddFrom("tblOrderMultiColumnReport", "MCol");               
                query.AddWhere("", "MCol.OrderID", "EQUALTO", campaignId.ToString());
               

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

