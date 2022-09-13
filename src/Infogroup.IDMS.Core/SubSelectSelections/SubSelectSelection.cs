using Infogroup.IDMS.SubSelects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.SubSelectSelections
{
	[Table("tblSubSelectSelection")]
    public class SubSelectSelection : Entity 
    {

		public virtual string cTableName { get; set; }
		
		[Required]
		public virtual string cJoinOperator { get; set; }
		
		[Required]
		public virtual string cQuestionFieldName { get; set; }
		
		[Required]
		public virtual string cQuestionDescription { get; set; }
		
		public virtual int iGroupNumber { get; set; }
		
		public virtual int iGroupOrder { get; set; }
		
		[Required]
		public virtual string cGrouping { get; set; }
		
		[Required]
		public virtual string cValues { get; set; }
		
		[Required]
		public virtual string cDescriptions { get; set; }
		
		[Required]
		public virtual string cValueMode { get; set; }
		
		public virtual string cValueOperator { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

		public virtual int SubSelectId { get; set; }
		
        [ForeignKey("SubSelectId")]
		public SubSelect SubSelectFk { get; set; }
		
    }
}