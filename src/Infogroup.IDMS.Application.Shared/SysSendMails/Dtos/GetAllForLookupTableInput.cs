using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.SysSendMails.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}