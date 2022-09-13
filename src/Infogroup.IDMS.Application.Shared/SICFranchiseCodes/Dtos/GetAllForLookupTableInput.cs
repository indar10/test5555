using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SICFranchiseCodes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}