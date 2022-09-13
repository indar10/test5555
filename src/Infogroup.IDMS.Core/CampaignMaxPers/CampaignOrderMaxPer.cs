using Infogroup.IDMS.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.CampaignMaxPers
{
	[Table("tblOrderMaxPer")]
    public class CampaignMaxPer : Entity 
    {

		[Required]
		public virtual string cGroup { get; set; }
		
		public virtual string cMaxPerField { get; set; }
		
		public virtual int? iMaxPerQuantity { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

		public virtual int OrderId { get; set; }
		
        [ForeignKey("OrderId")]
		public Campaign OrderFk { get; set; }
		
    }
}