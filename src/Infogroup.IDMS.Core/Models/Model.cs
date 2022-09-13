using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Models
{
	[Table("tblModel")]
    public class Model : Entity 
    {

		public virtual int DatabaseID { get; set; }
		
		[Required]
		public virtual string cModelName { get; set; }
		
		public virtual int iIntercept { get; set; }
		
		[Required]
		public virtual string cDescription { get; set; }
		
		[Required]
		public virtual string cModelNumber { get; set; }
		
		[Required]
		public virtual string cClientName { get; set; }
		
		public virtual bool iIsScoredForEveryBuild { get; set; }
		
		public virtual int nChildTableNumber { get; set; }
		
		public virtual bool iIsActive { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }

		[Required]
		public virtual string LK_ModelType { get; set; }

		[Required]
		public virtual string LK_GiftWeight { get; set; }
	}
}