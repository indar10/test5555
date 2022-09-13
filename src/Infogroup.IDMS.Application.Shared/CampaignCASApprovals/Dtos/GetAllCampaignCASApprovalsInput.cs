using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.CampaignCASApprovals.Dtos
{
    public class GetAllCampaignCASApprovalsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string CampaigncDatabaseNameFilter { get; set; }

		 
    }
}