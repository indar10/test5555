using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.GroupBrokers.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}