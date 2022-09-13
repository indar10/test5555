using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.SavedSelections.Dtos
{
    public class GetAllSavedSelectionsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public int CampaignID { get; set; }

    }
}