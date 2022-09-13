using Infogroup.IDMS.Segments;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.SubSelects
{
	[Table("tblSubSelect")]
    public class SubSelect : Entity 
    {

		[Required]
		public virtual string cIncludeExclude { get; set; }
		
		[Required]
		public virtual string cCompanyIndividual { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

		public virtual int SegmentId { get; set; }
		
        [ForeignKey("SegmentId")]
		public Segment SegmentFk { get; set; }
		
    }
}