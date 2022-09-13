using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.BuildTableLayouts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}