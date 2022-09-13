using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ProcessQueues
{
	[Table("tblProcessQueue")]
    public class ProcessQueue : Entity 
    {

		public virtual string cQueueName { get; set; }
		
		public virtual string cDescription { get; set; }
		
		public virtual int iAllowedThreadCount { get; set; }
		
		public virtual string LK_QueueType { get; set; }
		
		public virtual string LK_ProcessType { get; set; }
		
		public virtual bool iIsSuspended { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

    }
}