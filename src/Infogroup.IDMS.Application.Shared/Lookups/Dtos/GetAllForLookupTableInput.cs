using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Lookups.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}