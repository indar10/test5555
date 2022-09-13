using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SegmentPrevOrderses.Dtos
{
    public class SegmentPrevOrdersView
    {
        public int CurrentStatus { get; set; }
        public List<GetSegmentPrevOrdersForViewDto> listOfSegmentPrevOrders { get; set; }
    }
}
