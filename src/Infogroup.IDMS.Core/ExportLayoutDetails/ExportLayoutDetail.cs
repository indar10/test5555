using Infogroup.IDMS.ExportLayouts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ExportLayoutDetails
{
	[Table("tblExportLayoutDetail")]
    public class ExportLayoutDetail : Entity 
    {

		public virtual int iExportOrder { get; set; }
		
		public virtual string cOutputFieldName { get; set; }
		
		public virtual string cFieldDescription { get; set; }
		
		[Required]
		public virtual string cFieldName { get; set; }
		
		[Required]
		public virtual string cCalculation { get; set; }
		
		public virtual int? iWidth { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual string cTableNamePrefix { get; set; }
		
		[Required]
		public virtual string ctabledescription { get; set; }
		

		public virtual int ExportLayoutId { get; set; }

        public virtual bool? iIsCalculatedField { get; set; }
		
        [ForeignKey("ExportLayoutId")]
		public ExportLayout ExportLayoutFk { get; set; }
		
    }
}