using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Builds.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}