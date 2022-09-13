namespace Infogroup.IDMS.SegmentSelections.Dtos
{
    public class SaveGeoRadiusDto
    {
        public int MatchLevel { get; set; }
        public int CampaignId { get; set; }
        public int DatabaseId { get; set; }
        public SegmentSelectionDto Selection { get; set; }
    }
}