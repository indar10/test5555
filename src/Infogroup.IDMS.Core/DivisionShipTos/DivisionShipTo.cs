using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Infogroup.IDMS.Divisions;

namespace Infogroup.IDMS.DivisionShipTos
{
	[Table("tblDivisionShipTo")]
    public class DivisionShipTo : Entity 
    {

		public virtual int DivisionID { get; set; }
		
		[Required]
		public virtual string cCode { get; set; }
		
		[Required]
		public virtual string cCompany { get; set; }
		
		[Required]
		public virtual string cFirstName { get; set; }
		
		[Required]
		public virtual string cLastName { get; set; }
		
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
		public virtual string cCountry { get; set; }
		
		[Required]
		public virtual string cPhone { get; set; }
		
		[Required]
		public virtual string cFax { get; set; }
		
		[Required]
		public virtual string cEmail { get; set; }
		
		[Required]
		public virtual string cNotes { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		[Required]
		public virtual string cFTPServer { get; set; }
		
		public virtual string cUserID { get; set; }
		
		public virtual string cPassword { get; set; }
		
		public virtual bool iIsActive { get; set; }

        [ForeignKey("DivisionID")]
        public Division DivisionFk { get; set; }
    }
}