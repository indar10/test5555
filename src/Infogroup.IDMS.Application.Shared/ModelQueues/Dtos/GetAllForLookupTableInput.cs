using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ModelQueues.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}