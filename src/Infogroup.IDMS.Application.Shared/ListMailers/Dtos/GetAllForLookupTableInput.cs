using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ListMailers.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}