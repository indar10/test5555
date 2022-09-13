
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserReports.Dtos
{
    public class CreateOrEditUserReportDto : EntityDto<int?>
    {

		public int UserID { get; set; }
		
		public int ReportID { get; set; }
		
		
		 //public int? TblUserId { get; set; }
		 
		 //		 public int? ReportId { get; set; }
		 
		 
    }
}