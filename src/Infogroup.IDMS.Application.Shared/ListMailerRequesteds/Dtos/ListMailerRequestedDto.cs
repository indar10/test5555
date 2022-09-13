
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ListMailerRequesteds.Dtos
{
    public class ListMailerRequestedDto : EntityDto
    {
		public int MailerID { get; set; }

		public string cCreatedBy { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cModifiedBy { get; set; }

		public DateTime? dModifiedDate { get; set; }


		 public int ListID { get; set; }

		 
    }
}