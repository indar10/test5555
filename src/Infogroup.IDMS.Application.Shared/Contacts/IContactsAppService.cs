using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Contacts.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Contacts
{
    public interface IContactsAppService : IApplicationService 
    {
        Task<PagedResultDto<ContactDto>> GetAllContacts(GetAllContactsInput input, ContactType contactType);

		Task<CreateOrEditContactDto> GetContactForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditContactDto input);

    }
}