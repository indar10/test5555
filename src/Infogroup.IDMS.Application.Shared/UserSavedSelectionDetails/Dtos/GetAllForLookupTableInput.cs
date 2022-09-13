using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserSavedSelectionDetails.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}