using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}