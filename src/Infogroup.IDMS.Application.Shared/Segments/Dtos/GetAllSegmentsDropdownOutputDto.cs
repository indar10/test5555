using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Segments.Dtos
{
   public class GetAllSegmentsDropdownOutputDto
    {
        public List<DropdownOutputDto> SegmentDropDown { get; set; }
        public int CurrentStatus { get; set; }
        public bool IsQuickCountButtonVisible { get; set; }
        public bool IsSICScreenConfigured { get; set; }
        public bool IsOccupationScreenConfigured { get; set; }
        public bool IsCountyCityScreenConfigured { get; set; }
        public bool IsGeoSearchConfigured { get; set; }
        public string DefaultMaxPerValue { get; set; }        
    }
}
