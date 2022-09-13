using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.CampaignAttachments.Dtos
{
    public class GetAllCampaignAttachmentsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string CampaigncDescriptionFilter { get; set; }

		 
    }
}