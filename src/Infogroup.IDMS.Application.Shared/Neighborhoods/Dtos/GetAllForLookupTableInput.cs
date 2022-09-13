using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Neighborhoods.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}