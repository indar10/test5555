using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.UserReports.Dtos
{
    public class GetAllUserReportsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public int? MaxIDFilter { get; set; }
		public int? MinIDFilter { get; set; }

		public int? MaxReportIDFilter { get; set; }
		public int? MinReportIDFilter { get; set; }

		public string TBlUserIDFIlter { get; set; }
		public string TblUsercFirstNameFilter { get; set; }

		 		 public string ReportcReportNameFilter { get; set; }

		 
    }
}