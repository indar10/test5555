using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Contacts
{
	[Table("tblContact")]
    public class Contact : Entity 
    {

		public virtual int ContactID { get; set; }
		
		[Required]
		public virtual string cFirstName { get; set; }
		
		[Required]
		public virtual string cLastName { get; set; }
		
		[Required]
		public virtual string cTitle { get; set; }
		
		[Required]
		public virtual string cAddress1 { get; set; }
		
		[Required]
		public virtual string cAddress2 { get; set; }
		
		[Required]
		public virtual string cCity { get; set; }
		
		[Required]
		public virtual string cState { get; set; }
		
		[Required]
		public virtual string cZIP { get; set; }
		
		[Required]
		public virtual string cPhone1 { get; set; }
		
		[Required]
		public virtual string cPhone2 { get; set; }
		
		[Required]
		public virtual string cFax { get; set; }
		
		[Required]
		public virtual string cEmailAddress { get; set; }
		
		[Required]
		public virtual string cType { get; set; }

        [Required]
        public virtual bool iIsActive { get; set; }

        [Required]
        public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

    }
}