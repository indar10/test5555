using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ProcessQueues.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}