using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.MatchAppendOutputLayouts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}