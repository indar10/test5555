using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserDivisions.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}