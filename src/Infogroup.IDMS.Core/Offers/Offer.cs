using Infogroup.IDMS.Mailers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Offers
{
	[Table("tblOffer")]
    public class Offer : Entity 
    {

		[Required]
		public virtual string cOfferCode { get; set; }
		
		[Required]
		public virtual string cOfferName { get; set; }
		
		[Required]
		public virtual string LK_OfferType { get; set; }
		
		public virtual bool iIsActive { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual bool iHideInDWAP { get; set; }
		

		public virtual int MailerId { get; set; }
		
        [ForeignKey("MailerId")]
		public Mailer MailerFk { get; set; }
		
    }
}