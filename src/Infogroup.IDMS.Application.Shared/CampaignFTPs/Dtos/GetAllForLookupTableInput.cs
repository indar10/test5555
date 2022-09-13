using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.CampaignFTPs.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}