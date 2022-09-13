using Infogroup.IDMS.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.CampaignCASApprovals
{
	[Table("tblOrderCASApproval")]
    public class CampaignCASApproval : Entity 
    {

		public virtual int MasterLOLID { get; set; }
		
		[Required]
		public virtual string cStatus { get; set; }
		
		public virtual decimal nBasePrice { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }		

		public virtual int OrderId { get; set; }
		
        [ForeignKey("OrderId")]
		public Campaign OrderFk { get; set; }
		
    }
}