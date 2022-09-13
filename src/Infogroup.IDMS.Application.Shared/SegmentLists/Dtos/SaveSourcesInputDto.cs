using System.Collections.Generic;
namespace Infogroup.IDMS.SegmentLists.Dtos
{
    public class SaveSourcesInputDto
    {
		public int SegmentID { get; set; }
        public List<SourceDto> AddedSources { get; set; }
        public List<SourceDto> DeletedSources { get; set; }

    }
}