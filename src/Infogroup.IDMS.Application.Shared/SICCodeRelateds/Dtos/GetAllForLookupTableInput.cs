using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SICCodeRelateds.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}