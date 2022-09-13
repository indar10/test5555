using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.BuildTables.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}