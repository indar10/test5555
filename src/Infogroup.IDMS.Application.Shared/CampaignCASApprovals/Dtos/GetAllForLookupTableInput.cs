using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.CampaignCASApprovals.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}