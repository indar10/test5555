
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.ListMailerRequesteds.Dtos
{
    public class CreateOrEditListMailerRequestedDto : EntityDto<int?>
    {

		public int MailerID { get; set; }
		
		
		public string cCreatedBy { get; set; }
		
		
		public DateTime dCreatedDate { get; set; }
		
		
		public string cModifiedBy { get; set; }
		
		
		public DateTime? dModifiedDate { get; set; }
		
		
		 public int ListID { get; set; }
		

		public ActionType Action  { get; set; }
		 
		 
    }
}