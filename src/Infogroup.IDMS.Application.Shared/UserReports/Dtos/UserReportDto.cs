
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserReports.Dtos
{
    public class UserReportDto : EntityDto
    {
		public int ReportID { get; set; }


		 public int? TblUserId { get; set; }

		 		 public int? ReportId { get; set; }

		 
    }
}