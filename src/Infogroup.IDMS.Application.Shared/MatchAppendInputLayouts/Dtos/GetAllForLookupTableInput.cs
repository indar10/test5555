using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.MatchAppendInputLayouts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}