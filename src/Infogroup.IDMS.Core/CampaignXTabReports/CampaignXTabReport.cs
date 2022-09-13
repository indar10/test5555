using Infogroup.IDMS.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.CampaignXTabReports
{
	[Table("tblOrderXTabReport")]
    public class CampaignXTabReport : Entity 
    {

		public virtual string cXField { get; set; }
		
		public virtual string cYField { get; set; }
		
		public virtual bool iXTabBySegment { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		[Required]
		public virtual string cType { get; set; }

        public virtual string cSegmentNumbers { get; set; }

        public virtual int OrderId { get; set; }
		
        [ForeignKey("OrderId")]
		public Campaign OrderFk { get; set; }
		
    }
}