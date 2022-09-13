
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ListMailers.Dtos
{
    public class ListMailerDto : EntityDto
    {
		public int ID { get; set; }

		public int MailerID { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cCreatedBy { get; set; }

		public string cModifiedBy { get; set; }

		public DateTime? dModifiedDate { get; set; }


		 public int ListID { get; set; }

		 
    }
}