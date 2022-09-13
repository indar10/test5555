using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.DivisionMailers.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}