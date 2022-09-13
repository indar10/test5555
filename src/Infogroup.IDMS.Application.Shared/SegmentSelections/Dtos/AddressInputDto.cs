namespace Infogroup.IDMS.SegmentSelections.Dtos
{
    public class AddressInputDto
    {
        public int DatabaseId { get; set; }
        public int SegmentId { get; set; }
        public string MainTableName { get; set; }
        public string AddressFilter { get; set; }
    }
}