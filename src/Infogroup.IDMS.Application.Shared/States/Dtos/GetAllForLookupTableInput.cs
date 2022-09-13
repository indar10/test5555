using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.States.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}