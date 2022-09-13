using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.BatchQueues
{
	[Table("tblBatchQueue")]
    public class BatchQueue : Entity 
    {
        [Column("QueueId")]
        public override int Id { get; set; }

        public virtual int? ListId { get; set; }
		
		public virtual int? BuildId { get; set; }
		
		public virtual int DivisionId { get; set; }
		
		public virtual int ProcessTypeId { get; set; }
		
		public virtual int iStatusId { get; set; }
		
		public virtual int iPriority { get; set; }
		
		[Required]
		public virtual string cScheduledBy { get; set; }
		
		public virtual bool IsStopRequest { get; set; }
		
		public virtual string cStopRequestedBy { get; set; }
		
		public virtual int? DayDiff { get; set; }
		
		public virtual int? JoinKeyId { get; set; }
		
		public virtual string JoinKeyName { get; set; }
		
		public virtual int? ResponseLength { get; set; }
		
		public virtual int? FillerLength { get; set; }
		
		public virtual string FolderName { get; set; }
		
		public virtual string FileName { get; set; }
		
		public virtual string FieldName { get; set; }
		
		public virtual string Recipients { get; set; }
		
		public virtual DateTime dRecordDate { get; set; }
		
		public virtual DateTime? dScheduled { get; set; }
		
		public virtual DateTime? dStartDate { get; set; }
		
		public virtual DateTime? dEndDate { get; set; }
		
		public virtual DateTime? dStopped { get; set; }
		
		public virtual string ParmData { get; set; }
		
		public virtual string Result { get; set; }
		

    }
}