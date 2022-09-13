using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.IndustryCodes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}