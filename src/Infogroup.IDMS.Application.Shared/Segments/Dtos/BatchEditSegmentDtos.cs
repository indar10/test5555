using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class BatchEditSegmentDto : CreateOrEditSegmentDto
    {
        public int Index { get; set; }
        public bool Dirty { get; set; }
        public int NextStatus { get; set; }
        public int iDisplayOutputQty { get; set; }
    }
    public class GetInitialStateForBatchEdit
    {
        public bool IsCalculateDistanceSet { get; set; }
        public bool HasDefaultRules { get; set; }
        public List<DropdownOutputDto> MaxPers { get; set; }
    }

    public class SaveBatchSegmentDto
    {
        public List<CreateOrEditSegmentDto> ModifiedSegments  { get; set; }
        public int NextStatus { get; set; }
    }
}