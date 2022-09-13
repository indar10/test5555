using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ProcessQueueDatabases.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}