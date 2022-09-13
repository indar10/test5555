using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserDatabases.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}