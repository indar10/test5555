using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.OrderExportParts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}