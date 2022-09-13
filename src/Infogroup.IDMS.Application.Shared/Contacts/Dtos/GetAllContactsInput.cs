using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Contacts.Dtos
{
    public class GetAllContactsInput : PagedAndSortedResultRequestDto
    {
        public int ContactId { get; set; }

    }
}