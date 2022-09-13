using Infogroup.IDMS.Shared.Dtos;
using System;

namespace Infogroup.IDMS.SegmentPrevOrderses.Dtos
{
    public class GetSegmentPrevOrdersForViewDto
    {
        public string Description { get; set; }
        public string OrderNo { get; set; }
        public string Company { get; set; }
        public string cIndividualOrCompany { get; set; }
        public string cIncludeOrExclude { get; set; }
        public DateTime OrderStatusCreatedDate { get; set; }
        public ActionType action { get; set; }
        public int PreviousOrderID { get; set; }
        public int OrderID { get; set; }
        public string PrevSegmentIDs { get; set; }
        public string cLVAOrderNo { get; set; }
        public string cMatchFieldName { get; set; }
        public string cMatchFieldDescription { get; set; }

        public int BuildId { get; set; }

    }
}