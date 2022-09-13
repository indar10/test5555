using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}