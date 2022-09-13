using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class ImportSegmentDTO
    {
        public int iCopyToCampaignID { get; set; }
        public int iCopyFromCampaignID { get; set; }
        public string sCommaSeparatedSegments { get; set; }
        public string cSegmentDescription { get; set; }
        public int NumberOfSegments { get; set; }
        public bool isValid { get; set; }

    }
}
