using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.UserReports.Dtos
{
    public class GetUserReportForEditOutput
    {
		public CreateOrEditUserReportDto UserReport { get; set; }

		public string TblUsercFirstName { get; set;}

		public string ReportcReportName { get; set;}


    }
}