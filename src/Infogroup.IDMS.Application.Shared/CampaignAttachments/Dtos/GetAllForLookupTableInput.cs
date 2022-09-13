using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.CampaignAttachments.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}