using Infogroup.IDMS.Builds.Dtos;
using Infogroup.IDMS.Databases.Dtos;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class GetCampaignDropdownsDto
    {
        public GetAllDatabasesDropdownDto Databases { get; set; }
        public GetAllBuildDropdownOutputDto Builds{ get; set; }
        public bool DivisionalDatabase { get; set; }
        public DropdownOutputDto DefaultMailer { get; set; }
        public DropdownOutputDto DefaultBroker { get; set; }
        public int? OfferID { get; set; }

        public bool isShowCustomer { get; set; }
    }
}