using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.OrderExportParts.Dtos
{
    public class GetAllOrderExportPartsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string CampaigncDescriptionFilter { get; set; }

		 
    }
}