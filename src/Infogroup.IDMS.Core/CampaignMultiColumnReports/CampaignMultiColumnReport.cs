using Infogroup.IDMS.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.CampaignMultiColumnReports
{
	[Table("tblOrderMultiColumnReport")]
    public class CampaignMultiColumnReport : Entity 
    {

		[Required]
		public virtual string cFields { get; set; }

        [Required]
        public virtual string cFieldsDescription { get; set; }

        public virtual bool iMultiColBySegment { get; set; }
		
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