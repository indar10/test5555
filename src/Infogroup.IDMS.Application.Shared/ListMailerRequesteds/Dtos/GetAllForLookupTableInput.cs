using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ListMailerRequesteds.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}