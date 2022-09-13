using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.IDMSConfigurations
{
	[Table("tblConfiguration")]
    public class IDMSConfiguration : Entity
    {		
		public virtual int DivisionID { get; set; }
		
		public virtual int DatabaseID { get; set; }
		
		[Required]
		public virtual string cItem { get; set; }
		
		[Required]
		public virtual string cDescription { get; set; }
		
		[Required]
		public virtual string cValue { get; set; }
		
		public virtual int iValue { get; set; }
		
		public virtual DateTime? dValue { get; set; }
		
		[Required]
		public virtual string mValue { get; set; }
		
		public virtual bool iIsActive { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual bool iIsEncrypted { get; set; }
		

    }
}