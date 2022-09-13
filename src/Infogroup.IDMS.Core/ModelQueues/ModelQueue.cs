using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ModelQueues
{
	[Table("tblModelQueue")]
    public class ModelQueue : Entity 
    {

		public virtual int ModelDetailID { get; set; }
		
		[Required]
		public virtual string LK_ModelStatus { get; set; }
		
		public virtual bool iIsCurrent { get; set; }
		
		public virtual bool iIsSampleScore { get; set; }
		
		public virtual DateTime? dScheduledDate { get; set; }
		
		public virtual int iPriority { get; set; }
		
		public virtual DateTime? dStartTime { get; set; }
		
		public virtual DateTime? dEndTime { get; set; }
		
		[Required]
		public virtual string cNotes { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

    }
}