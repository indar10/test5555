using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserAccessObjects.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}