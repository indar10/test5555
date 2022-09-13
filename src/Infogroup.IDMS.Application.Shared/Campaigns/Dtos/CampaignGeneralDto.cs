using Infogroup.IDMS.Shared.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class CampaignGeneralDto
    {
        [Required]
        [StringLength(50)]
        public string cDescription { get; set; }
        [StringLength(50)]
        public string cOfferName { get; set; }
        [StringLength(1)]
        public string cOrderType { get; set; }
        public int DatabaseID { get; set; }
        public int BuildID { get; set; }
        public int OfferID { get; set; }
        public DropdownOutputDto Mailer { get; set; }
        public DropdownOutputDto Broker { get; set; }
        public string MailerDescription { get; set; }
        public string BrokerDescription { get; set; }
        [StringLength(1)]
        public string cChannelType { get; set; }
        public bool HasUserMailer { get; set; }
        public bool DivisionalDatabase { get; set; }

        public int DivisionID { get; set; }
    }
}
