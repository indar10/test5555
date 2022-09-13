using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class GetSegmentListForView
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string cMaxPerGroup { get; set; }
        public int iProvidedQty { get; set; }
        public int iRequiredQty { get; set; }
        public int iOutputQty { get; set; }
        public int iAvailableQty { get; set; }
        public int iDedupeOrderSpecified { get; set; }
        public string cKeyCode1 { get; set; }
        public string cKeyCode2 { get; set; }
        public string cFieldDescription { get; set; }
        public string cFieldName { get; set; }
        public int OrderId { get; set; }
        public int? iGroup { get; set; }

    }
}
