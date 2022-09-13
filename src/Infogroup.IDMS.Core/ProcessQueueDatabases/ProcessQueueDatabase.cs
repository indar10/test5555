using Infogroup.IDMS.ProcessQueues;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ProcessQueueDatabases
{
	[Table("tblProcessQueueDatabase")]
    public class ProcessQueueDatabase : Entity 
    {

		public virtual int DatabaseId { get; set; }
		
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

		public virtual int ProcessQueueId { get; set; }
		
        [ForeignKey("ProcessQueueId")]
		public ProcessQueue ProcessQueueFk { get; set; }
		
    }
}