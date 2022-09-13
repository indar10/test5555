using Abp.Domain.Repositories;
using Infogroup.IDMS.Contacts.Dtos;
using System.Collections.Generic;


namespace Infogroup.IDMS.Contacts
{
    public interface IContactRepository : IRepository<Contact, int>
    {
        List<CreateOrEditContactDto> GetContacts(int Id, ContactType contactType,ContactTableType contactTableType);
    }
}
