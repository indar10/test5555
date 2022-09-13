using Infogroup.IDMS.UserSavedSelections;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.UserSavedSelectionDetails
{
	[Table("tblUserSavedSelectionDetail")]
    public class UserSavedSelectionDetail : Entity 
    {

		public virtual string cQuestionFieldName { get; set; }
		
		public virtual string cQuestionDescription { get; set; }
		
		[Required]
		public virtual string cJoinOperator { get; set; }
		
		public virtual int iGroupNumber { get; set; }
		
		public virtual int iGroupOrder { get; set; }
		
		[Required]
		public virtual string cGrouping { get; set; }
		
		[Required]
		public virtual string cValues { get; set; }
		
		[Required]
		public virtual string cValueMode { get; set; }
		
		[Required]
		public virtual string cDescriptions { get; set; }
		
		public virtual string cValueOperator { get; set; }
		
		public virtual bool iIsActive { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual string cTableName { get; set; }
		

		public virtual int UserSavedSelectionId { get; set; }
		
        [ForeignKey("UserSavedSelectionId")]
		public UserSavedSelection UserSavedSelectionFk { get; set; }
		
    }
}