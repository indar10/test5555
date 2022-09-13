using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.CampaignExportLayouts.Dtos
{
    public class GetAllCampaignExportLayoutsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string CampaigncDescriptionFilter { get; set; }

		 
    }
}