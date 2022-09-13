using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserSavedSelections.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}