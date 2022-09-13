using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.CampaignMaxPers.Dtos;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Campaigns
{
    public interface ICampaignsAppService : IApplicationService
    {
        Task<PagedResultDto<GetCampaignsListForView>> GetAllCampaignsList(GetCampaignListFilters input);
        FileDto DownloadOutputLayoutTemplateExcelTest(int campaignId);
        List<DropdownOutputDto> GetMaxPerFieldDropdownData(int? databaseId, int? buildId, string Filter);
        Dictionary<string, List<string>> getSegmentsWithSourcesAndSubSelect(int campaignId);
        CampaignLevelMaxPerDto FetchOrderLevelMaxPer(int campaignId);
        void UpdateOrderStatus(CampaignActionInputDto input);
    }
}