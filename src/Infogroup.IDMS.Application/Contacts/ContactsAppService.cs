using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Contacts.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.Sessions;

namespace Infogroup.IDMS.Contacts
{
    [AbpAuthorize(AppPermissions.Pages_OwnerContacts, AppPermissions.Pages_ManagerContacts, AppPermissions.Pages_MailerContacts, AppPermissions.Pages_ContactsBrokers)]
    public class ContactsAppService : IDMSAppServiceBase, IContactsAppService
    {
        private readonly IRepository<Contact> _contactRepository;
        private readonly string _cLastNameAscString = " cLastName asc";
        private readonly AppSession _mySession;

        public ContactsAppService(IRepository<Contact> contactRepository,
             AppSession mySession
             )
        {
            _contactRepository = contactRepository;
            _mySession = mySession;
        }

        #region Fetch Contacts
        public async Task<PagedResultDto<ContactDto>> GetAllContacts(GetAllContactsInput input, ContactType contactType)
        {
            try
            {
                var filteredContacts = _contactRepository.GetAll().Where(e => e.cType == Convert.ToInt32(contactType).ToString())
                        .WhereIf(input.ContactId > -1, e => Convert.ToInt32(e.ContactID) == input.ContactId);


                var pagedAndFilteredContacts = filteredContacts
                    .OrderBy(input.Sorting ?? _cLastNameAscString)
                    .PageBy(input);

                var contacts = from o in pagedAndFilteredContacts
                               select  ObjectMapper.Map<ContactDto>(o);

                var totalCount = await filteredContacts.CountAsync();

                return new PagedResultDto<ContactDto>(
                    totalCount,
                    await contacts.ToListAsync()
                );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CreateOrEditContactDto> GetContactForEdit(EntityDto input)
        {
            try
            {

                var contact = await _contactRepository.FirstOrDefaultAsync(input.Id);
                var editContactData = ObjectMapper.Map<CreateOrEditContactDto>(contact);
                return editContactData;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Save Contacts
        [AbpAuthorize(AppPermissions.Pages_OwnerContacts_Create, AppPermissions.Pages_OwnerContacts_Edit, AppPermissions.Pages_ManagerContacts_Create, AppPermissions.Pages_ManagerContacts_Edit, AppPermissions.Pages_MailerContacts_Create, AppPermissions.Pages_MailerContacts_Edit,AppPermissions.Pages_ContactsBrokers_Create, AppPermissions.Pages_ContactsBrokers_Edit)]
        public async Task CreateOrEdit(CreateOrEditContactDto input)
        {

            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                if (input.Id == null)
                {
                    input.cType = Convert.ToInt32(input.contactType).ToString();
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var contact = ObjectMapper.Map<Contact>(input);
                    await _contactRepository.InsertAsync(contact);
                }
                else
                {
                    var updateContact = _contactRepository.Get(input.Id.GetValueOrDefault());
                    input.ContactID = updateContact.ContactID;
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updateContact);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }
}