using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ExternalBuildTableDatabases.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}