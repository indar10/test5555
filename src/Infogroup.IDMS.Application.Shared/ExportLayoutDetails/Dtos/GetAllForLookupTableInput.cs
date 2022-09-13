using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ExportLayoutDetails.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}