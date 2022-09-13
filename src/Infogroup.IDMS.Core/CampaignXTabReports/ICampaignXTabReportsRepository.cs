using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.CampaignXTabReports;
using Infogroup.IDMS.CampaignXTabReports.Dtos;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.CampaignXTabReports
{
    public interface ICampaignXTabReportsRepository : IRepository<CampaignXTabReport, int>
    {
        List<GetCampaignXTabReportsListForView> GetAllCampaignXtabReports(int campaignID, int databaseID);
        List<int> GetAllCampaignXtabReportIds(int campaignID);
    }
}
