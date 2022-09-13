
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ProcessQueues.Dtos
{
    public class ProcessQueueDto : EntityDto
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
		public string queueDescription { get; set; }
		public string processDescription { get; set; }





    }
}