using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.CampaignMultiColumnReports.Dtos
{
    public class GetAllCampaignMultiColumnReportsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string CampaigncDescriptionFilter { get; set; }

		 
    }
}