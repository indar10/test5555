using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Contacts.Dtos
{
    public class ContactDto : EntityDto
    {
		public string cFirstName { get; set; }

		public string cLastName { get; set; }

		public string cEmailAddress { get; set; }

		public bool iIsActive { get; set; }

    }
}