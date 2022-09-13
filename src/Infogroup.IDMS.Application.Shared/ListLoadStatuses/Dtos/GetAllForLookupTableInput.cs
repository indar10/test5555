using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ListLoadStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}