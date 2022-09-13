using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SICCodes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}