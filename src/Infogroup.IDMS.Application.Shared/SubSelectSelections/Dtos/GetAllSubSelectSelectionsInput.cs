using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.SubSelectSelections.Dtos
{
    public class GetAllSubSelectSelectionsInput 
    {
		public int iSubSelectID { get; set; }
		public int SegmentId { get; set; }
		public int BuildId { get; set; }
		public string isSegment { get; set; }
		public string SubSelectcIncludeExcludeFilter { get; set; }

		 
    }
}