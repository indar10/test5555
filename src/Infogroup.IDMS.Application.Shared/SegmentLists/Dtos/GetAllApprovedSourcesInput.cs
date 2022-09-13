using System.Collections.Generic;

namespace Infogroup.IDMS.SegmentLists.Dtos
{
    public class GetAllApprovedSourcesInput
    {
		public string Filter { get; set; }
        public int SegmentID { get; set;}
        public List<int> UnsavedListIDs { get; set; } = new List<int>();
    }
}