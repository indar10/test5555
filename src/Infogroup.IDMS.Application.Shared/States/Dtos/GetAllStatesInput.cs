using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.States.Dtos
{
    public class GetAllStatesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }

}