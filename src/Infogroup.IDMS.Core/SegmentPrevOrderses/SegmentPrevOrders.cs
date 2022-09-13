using Infogroup.IDMS.Segments;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.SegmentPrevOrderses
{
	[Table("tblSegmentPrevOrders")]
    public class SegmentPrevOrders : Entity 
    {

		public virtual int PrevOrderID { get; set; }
		
		[Required]
		public virtual string cIncludeExclude { get; set; }
		
		[Required]
		public virtual string cPrevSegmentID { get; set; }
		
		[Required]
		public virtual string cPrevSegmentNumber { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		[Required]
		public virtual string cCompareFieldName { get; set; }
		

		public virtual int SegmentId { get; set; }
		
        [ForeignKey("SegmentId")]
		public Segment SegmentFk { get; set; }

        public string cMatchFieldName { get; set; }


    }
}