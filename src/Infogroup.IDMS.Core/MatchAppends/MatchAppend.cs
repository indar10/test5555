using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.MatchAppends
{
	[Table("tblMatchAppend")]
    public class MatchAppend : Entity 
    {

		public virtual int DatabaseID { get; set; }
		
		public virtual int BuildID { get; set; }
		
		[Required]
		public virtual string UploadFilePath { get; set; }
		
		[Required]
		public virtual string LK_FileType { get; set; }
		
		[Required]
		public virtual string LK_ExportFileFormatID { get; set; }
		
		[Required]
		public virtual string cOrderType { get; set; }
		
		[Required]
		public virtual string cKeyCode { get; set; }
		
		public virtual bool iSkipFirstRow { get; set; }
		
		[Required]
		public virtual string cClientName { get; set; }
		
		[Required]
		public virtual string cRequestReason { get; set; }
		
		[Required]
		public virtual string cSourceFilter { get; set; }
		
		[Required]
		public virtual string cInputFilter { get; set; }
		
		[Required]
		public virtual string cIDMSMatchFieldName { get; set; }
		
		[Required]
		public virtual string cInputMatchFieldName { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		[Required]
		public virtual string cClientFileName { get; set; }
		
		public virtual int iExportType { get; set; }
		

    }
}