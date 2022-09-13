using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignMultiColumnReports;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.CampaignMultiColumnReports
{
    public interface ICampaignMultiColumnReportRepository : IRepository<CampaignMultiColumnReport, int>
    {
        List<GetCampaignMultidimensionalReportForViewDto> GetAllCampaignMultiDimensionalReports(string Query, List<SqlParameter> sqlParameters);
    }
}
