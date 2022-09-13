using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}