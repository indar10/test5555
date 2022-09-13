using Infogroup.IDMS.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.OrderExportParts
{
	[Table("tblOrderExportPart")]
    public class OrderExportPart : Entity 
    {

		[Required]
		public virtual string cPartNo { get; set; }
		
		public virtual int SegmentID { get; set; }
		
		public virtual int iQuantity { get; set; }
		
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