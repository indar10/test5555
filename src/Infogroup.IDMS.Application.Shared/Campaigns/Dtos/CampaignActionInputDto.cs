namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class CampaignActionInputDto
    {
        public int CampaignId { get; set; }
        public int DatabaseId { get; set; }
        public int CampaignStatus { get; set; }
        public int BuildId { get; set; }
        public int CurrentBuild { get; set; }
        public bool IsExecute { get; set; } = false;
        public string CNotes{ get; set; } = "";
    }
}