using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.DivisionBrokers.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}