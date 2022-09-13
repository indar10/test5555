using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class CampaignCopyDto
    {
        public int CampaignId { get; set; }
        public DropdownOutputDto Mailer { get; set; }
        public int BrokerId { get; set; }
        public string cDescription { get; set; }
        public string cOfferName { get; set; }
        public int DatabaseId { get; set; }
        public int BuildId { get; set; }
        public int OfferId { get; set; }
        public DropdownOutputDto DivisionalMailer { get; set; }
        public DropdownOutputDto DivisionalBroker { get; set; }
        public bool DivisionalDatabase { get; set; }
        public int NumberOfCopies { get; set; }
    }
}
