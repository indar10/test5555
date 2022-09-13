using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Occupations.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}