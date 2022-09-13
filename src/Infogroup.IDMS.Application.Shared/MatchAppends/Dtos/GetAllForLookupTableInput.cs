using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.MatchAppends.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}