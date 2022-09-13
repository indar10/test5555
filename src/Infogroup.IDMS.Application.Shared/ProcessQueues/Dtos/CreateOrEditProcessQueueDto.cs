
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.ProcessQueues.Dtos
{
    public class CreateOrEditProcessQueueDto : EntityDto<int?>
    {

		public string cQueueName { get; set; }
		
		
		public string cDescription { get; set; }
		
		
		public int iAllowedThreadCount { get; set; }
		
		
		public string LK_QueueType { get; set; }
		
		
		public string LK_ProcessType { get; set; }
		
		
		public bool iIsSuspended { get; set; }
		
		
		public DateTime dCreatedDate { get; set; }
		
		
		public string cCreatedBy { get; set; }
		
		
		public DateTime? dModifiedDate { get; set; }
		
		
		public string cModifiedBy { get; set; }
		
		

    }
}