using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Reports.Dtos
{
    public class GetReportForViewDto
    {        
		public List<DropdownOutputDto> ReportOptions { get; set; }
        public int SelectedReport { get; set; }
        public List<ReportDto> Reports { get; set; }   
        public string AccessToken { get; set; }

    }
}