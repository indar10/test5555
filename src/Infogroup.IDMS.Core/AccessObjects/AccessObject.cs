using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.AccessObjects
{
	[Table("tblAccessObject")]
    public class AccessObject : Entity 
    {

		[Required]
		public virtual string cCode { get; set; }
		
		[Required]
		public virtual string cDescription { get; set; }
		
		public virtual int iMainMenuID { get; set; }
		
		public virtual int iOrderID { get; set; }
		
		[Required]
		public virtual string cPath { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

    }
}