using Infogroup.IDMS.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.CampaignDecoys
{
	[Table("tblOrderDecoy")]
    public class CampaignDecoy : Entity 
    {

		[Required]
		public virtual string cFirstName { get; set; }
		
		[Required]
		public virtual string cLastName { get; set; }
		
		[Required]
		public virtual string cName { get; set; }
		
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
		public virtual string cCompany { get; set; }
		
		[Required]
		public virtual string cTitle { get; set; }
		
		[Required]
		public virtual string cEmail { get; set; }
		
		[Required]
		public virtual string cPhone { get; set; }
		
		[Required]
		public virtual string cFax { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual string cDecoyType { get; set; }
		
		public virtual string cZip4 { get; set; }
		
		public virtual string cKeyCode1 { get; set; }
		
		public virtual string cDecoyGroup { get; set; }
		

		public virtual int OrderId { get; set; }
		
        [ForeignKey("OrderId")]
		public Campaign OrderFk { get; set; }
		
    }
}