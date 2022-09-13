using Infogroup.IDMS.Databases;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Infogroup.IDMS.Owners
{
	[Table("tblOwner")]
    public class Owner : Entity
    {
		[Required]
		public virtual string cCode { get; set; }
		
		[Required]
		public virtual string cCompany { get; set; }
		
		[Required]
		public virtual string cAddress1 { get; set; }
		
		[Required]
		public virtual string cAddress2 { get; set; }
		
		[Required]
		public virtual string cCity { get; set; }
		
		[Required]
		public virtual string cState { get; set; }
		
		[Required]
		public virtual string cZip { get; set; }
		
		[Required]
		public virtual string cPhone { get; set; }
		
		[Required]
		public virtual string cFax { get; set; }
		
		[Required]
		public virtual string cNotes { get; set; }

        [Required]
        public virtual bool iIsActive { get; set; }

        [Required]
        //[Column("dCreatedDate")]
        public virtual DateTime dCreatedDate { get; set; }

        [Required]
        public virtual string cCreatedBy { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual int DatabaseId { get; set; }
		
        [ForeignKey("DatabaseId")]
		public Database DatabaseFk { get; set; }      
    }
}