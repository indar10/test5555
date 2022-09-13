using System.ComponentModel.DataAnnotations;
namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class CampaignDistinctFields
    {
        public int BuildID { get; set; }
        [StringLength(1)]
        public string cChannelType { get; set; }
        [StringLength(50)]
        public string cOfferName { get; set; }
        public int DivisionMailerID { get; set; }
        public int DivisionBrokerID { get; set; }
        public int MailerID { get; set; }
        public int OfferID { get; set; }
    }
}
