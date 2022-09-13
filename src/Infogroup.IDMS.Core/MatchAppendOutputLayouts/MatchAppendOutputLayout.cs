using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.MatchAppendOutputLayouts
{
	[Table("tblMatchAppendOutputLayout")]
    public class MatchAppendOutputLayout : Entity 
    {

		public virtual int MatchAppendID { get; set; }
		
		[Required]
		public virtual string cTableName { get; set; }
		
		[Required]
		public virtual string cFieldName { get; set; }
		
		public virtual int iOutputLength { get; set; }
		
		[Required]
		public virtual string cOutputFieldName { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual int iOutputLayoutOrder { get; set; }
		

    }
}