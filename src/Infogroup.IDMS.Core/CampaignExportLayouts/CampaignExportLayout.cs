using Infogroup.IDMS.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.CampaignExportLayouts
{
	[Table("tblOrderExportLayout")]
    public class CampaignExportLayout : Entity 
    {

		[Required]
		public virtual string cFieldName { get; set; }
		
		public virtual int iExportOrder { get; set; }
		
		[Required]
		public virtual string cCalculation { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual int? iWidth { get; set; }
		
		public virtual string cOutputFieldName { get; set; }
		
		public virtual string cTableNamePrefix { get; set; }
		

		public virtual int OrderId { get; set; }
        public virtual bool? iIsCalculatedField { get; set; }

        [ForeignKey("OrderId")]
		public Campaign OrderFk { get; set; }
		
    }
}