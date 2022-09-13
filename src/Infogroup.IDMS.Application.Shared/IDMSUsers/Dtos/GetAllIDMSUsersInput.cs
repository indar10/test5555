using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.IDMSUsers.Dtos
{
    public class GetAllIDMSUsersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

    }
}