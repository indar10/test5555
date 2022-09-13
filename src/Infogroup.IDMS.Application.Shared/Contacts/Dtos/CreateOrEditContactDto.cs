using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Contacts.Dtos
{
    public class CreateOrEditContactDto : EntityDto<int?>
    {
        public int ContactID { get; set; }

        public string cCompany { get; set; }

        public string cFirstName { get; set; }

        public string cLastName { get; set; }

        public string cTitle { get; set; }

        public string cAddress1 { get; set; }

        public string cAddress2 { get; set; }

        public string cCity { get; set; }

        public string cState { get; set; }

        public string cZIP { get; set; }

        public string cPhone1 { get; set; }

        public string cPhone2 { get; set; }

        public string cFax { get; set; }

        public string cEmailAddress { get; set; }

        public bool iIsActive { get; set; }

        public string cType { get; set; }

        public string cCreatedBy { get; set; }

        public string cModifiedBy { get; set; }

        public DateTime dCreatedDate { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public ContactType contactType { get; set; }

    }
}