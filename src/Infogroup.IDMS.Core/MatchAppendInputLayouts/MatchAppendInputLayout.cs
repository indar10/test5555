using Infogroup.IDMS.MatchAppends;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.MatchAppendInputLayouts
{
	[Table("tblMatchAppendInputLayout")]
    public class MatchAppendInputLayout : Entity 
    {

		[Required]
		public virtual string cFieldName { get; set; }
		
		public virtual int iStartIndex { get; set; }
		
		public virtual int iEndIndex { get; set; }
		
		public virtual int iDataLength { get; set; }
		
		public virtual int iImportLayoutOrder { get; set; }
		
		[Required]
		public virtual string cMCMapping { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

		public virtual int MatchAppendId { get; set; }
		
        [ForeignKey("MatchAppendId")]
		public MatchAppend MatchAppendFk { get; set; }
		
    }
}