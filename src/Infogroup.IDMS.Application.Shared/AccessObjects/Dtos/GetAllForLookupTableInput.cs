using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.AccessObjects.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}