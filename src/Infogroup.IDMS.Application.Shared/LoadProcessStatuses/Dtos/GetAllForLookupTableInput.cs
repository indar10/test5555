using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.LoadProcessStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}