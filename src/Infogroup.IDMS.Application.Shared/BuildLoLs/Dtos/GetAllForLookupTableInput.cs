using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.BuildLoLs.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}