using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Owners.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}