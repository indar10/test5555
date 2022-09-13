using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.AutoSuppresses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}