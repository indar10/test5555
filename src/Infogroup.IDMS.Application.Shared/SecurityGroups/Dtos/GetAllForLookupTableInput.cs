using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SecurityGroups.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}