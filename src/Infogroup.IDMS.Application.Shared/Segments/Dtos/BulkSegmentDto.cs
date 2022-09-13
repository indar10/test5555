namespace Infogroup.IDMS.Segments.Dtos
{
    public class BulkSegmentDto
    {
        public int CampaignId { get; set; }
        public bool IsDefaultFormat { get; set; }
        public bool IsPopulateKeycode { get; set; }
        public bool IsGroupByKeyCode1 { get; set; }
        public string Path { get; set; }
        public int DatabaseId { get; set; }
    }
}