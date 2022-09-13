using Infogroup.IDMS.SubSelects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.SubSelectLists
{
	[Table("tblSubSelectList")]
    public class SubSelectList : Entity 
    {

		public virtual int MasterLOLID { get; set; }
		
		public virtual bool iIsHouseList { get; set; }
		
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