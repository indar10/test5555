using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.CampaignXTabReports.Dtos
{
    public class GetCampaignXTabReportsListForView
    {
        public string cXField { get; set; }
        public string cYField { get; set; }
        public string IsXTab { get; set; }
        public string cType { get; set; }
        public int ID { get; set; }
        public string cXDesc { get; set; }
        public string cYDesc { get; set; }
        public bool iXTabBySegment { get; set; }
        public string cTypeName { get; set; }
        public string cSegmentNumbers { get; set; }
        public ActionType Action;
    }

    public class GetXtabReportsDataDto
    { 
        public List<GetCampaignXTabReportsListForView> XtabRecords { get; set; }

        public List<DropdownOutputDto> XFieldDropdown;

        public List<DropdownOutputDto> YFieldDropdown;
    }

}
