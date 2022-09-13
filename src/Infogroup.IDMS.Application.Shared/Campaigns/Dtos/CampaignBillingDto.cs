using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class CampaignBillingDto
    {
        [StringLength(25)]
        public string LVAOrderNo { get; set; }
        public bool? IsNoUsage { get; set; }
        public bool IsNetUse { get; set; }
        [StringLength(15)]
        public string NextMarkOrderNo { get; set; }
        [StringLength(30)]
        public string BrokerPONo { get; set; }
        [StringLength(30)]
        public string SANNumber { get; set; }
    }
}
