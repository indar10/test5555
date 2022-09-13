using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserGroups.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}