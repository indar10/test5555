using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ListAutomate.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}