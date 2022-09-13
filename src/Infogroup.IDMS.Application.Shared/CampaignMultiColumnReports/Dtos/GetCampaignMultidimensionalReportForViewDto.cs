using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.CampaignMultiColumnReports.Dtos
{
    public class GetCampaignMultidimensionalReportForViewDto
    {
       public string cFieldName { get; set; }

        public string[] cFields { get; set; }
        public string cFieldDescription { get; set; }
        public string cType { get; set; }
        public int ID { get; set; }    
        public string IsMulti { get; set; }
        public bool IMultiBySegment { get; set; }
        public string cTypeName { get; set; }
        public string cSegmentNumbers { get; set; }
        public ActionType Action;
        public int ExtBuildId { get; set; }
        public string cFieldType { get; set; }

        public string cTableName { get; set; }
    }

    public class GetMultidimensionalReportsDataDto
    { 
        public List<GetCampaignMultidimensionalReportForViewDto> multidimensionalRecords { get; set; }

        public List<DropdownOutputDto> fieldsDropdown;

       
    }

}
