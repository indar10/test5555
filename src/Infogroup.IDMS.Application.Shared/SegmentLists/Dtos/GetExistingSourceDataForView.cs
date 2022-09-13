using System.Collections.Generic;
namespace Infogroup.IDMS.SegmentLists.Dtos
{
    public class GetExistingSourceDataForView
    {
        public int iDedupeOrderSpecified { get; set; }
        public int CurrentStatus { get; set; }
        public bool CampaignLevel { get; set; } 
        public List<SourceDto> SelectedSources { get; set; }
    }
}
