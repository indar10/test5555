using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SegmentSelections.Dtos
{
    public class GetQueryBuilderDetails
    {
        public string FilterDetails { get; set; }
        public string FilterQuery { get; set; }
        public string UnMappedFilters { get; set; }
        public int BuildLolId { get; set; }
    }
}
