using Infogroup.IDMS.Databases;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ExportLayouts
{
	[Table("tblExportLayout")]
    public class ExportLayout : Entity 
    {

		[Required]
		public virtual string cDescription { get; set; }
		
		public virtual bool iHasPhone { get; set; }
		
		public virtual bool iIsActive { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual int? GroupID { get; set; }
		
		public virtual bool iHasKeyCode { get; set; }
		
		[Required]
		public virtual string cOutputCase { get; set; }
		

		public virtual int? DatabaseId { get; set; }
		
        [ForeignKey("DatabaseId")]
		public Database DatabaseFk { get; set; }
		
    }
}