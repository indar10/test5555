namespace Infogroup.IDMS.Segments.Dtos
{
    public class GetSegmentForView
    {
		public SegmentDto Segment { get; set; }

		public string OrdercDatabaseName { get; set;}

        public int OrderId { get; set; }

        public int SegmentId { get; set; }

    }
}