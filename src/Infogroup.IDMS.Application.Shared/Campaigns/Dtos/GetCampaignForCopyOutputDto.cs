using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class GetCampaignForCopyOutputDto
    {
        public List<DropdownOutputDto> Builds { get; set; }
        public List<DropdownOutputDto> Offers { get; set; }
        public int UserDatabaseMailerRecordCount { get; set; }
        public CampaignCopyDto CampaignCopyData { get; set; }
    }
}
