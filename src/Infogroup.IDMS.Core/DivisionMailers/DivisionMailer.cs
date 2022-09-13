using Infogroup.IDMS.Divisions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.DivisionMailers
{
	[Table("tblDivisionMailer")]
    public class DivisionMailer : Entity 
    {

		[Required]
		public virtual string cCode { get; set; }
		
		[Required]
		public virtual string cCompany { get; set; }
		
		[Required]
		public virtual string cFirstName { get; set; }
		
		[Required]
		public virtual string cLastName { get; set; }
		
		[Required]
		public virtual string cAddr1 { get; set; }
		
		[Required]
		public virtual string cAddr2 { get; set; }
		
		[Required]
		public virtual string cAddr3 { get; set; }
		
		[Required]
		public virtual string cCity { get; set; }
		
		[Required]
		public virtual string cState { get; set; }
		
		[Required]
		public virtual string cZip { get; set; }
		
		[Required]
		public virtual string cCountry { get; set; }
		
		public virtual string cPhone { get; set; }
		
		[Required]
		public virtual string cFax { get; set; }
		
		[Required]
		public virtual string cEmail { get; set; }
		
		[Required]
		public virtual string mNotes { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual bool iIsActive { get; set; }
		

		public virtual int DivisionId { get; set; }
		
        [ForeignKey("DivisionId")]
		public Division DivisionFk { get; set; }
		
    }
}