using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.BatchQueues.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}