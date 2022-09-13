using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.MatchAppendStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}