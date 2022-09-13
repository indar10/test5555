using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.CampaignMaxPers.Dtos
{
    public class GetCampaignMaxPerForViewDto
    {
        public List<SegmentLevelMaxPerDto> GetSegmentLevelMaxPerData { get; set; }

        public List<DropdownOutputDto> GetMaxPerFieldDropdownData { get; set; }

        public CampaignLevelMaxPerDto GetCampaignLevelMaxPerData { get; set; }

        public int CampaignId { get; set; }
    }
}