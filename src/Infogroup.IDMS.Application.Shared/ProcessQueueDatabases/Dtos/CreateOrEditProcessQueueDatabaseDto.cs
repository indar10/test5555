
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ProcessQueueDatabases.Dtos
{
    public class CreateOrEditProcessQueueDatabaseDto : EntityDto<int?>
    {

		public int DatabaseId { get; set; }
		
		
		public string cCreatedBy { get; set; }
		
		public DateTime dCreatedDate { get; set; }
		
		
		public string cModifiedBy { get; set; }
		
		
		public DateTime? dModifiedDate { get; set; }
		
		
		 public int ProcessQueueId { get; set; }
		 
		 
    }
}