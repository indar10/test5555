using Infogroup.IDMS.Offers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.OfferSamples
{
	[Table("tblOfferSample")]
    public class OfferSample : Entity 
    {

		[Required]
		public virtual string cDescription { get; set; }
		
		public virtual string cFileName { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

		public virtual int OfferId { get; set; }
		
        [ForeignKey("OfferId")]
		public Offer OfferFk { get; set; }
		
    }
}