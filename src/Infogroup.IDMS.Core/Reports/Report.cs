using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Reports
{
	[Table("tblReport")]
    public class Report : Entity
    { 		
		[Required]
		public virtual string cReportID { get; set; }
		
		[Required]
		public virtual string cReportName { get; set; }
		
		[Required]
		public virtual string cReportWorkSpaceID { get; set; }
		
		[Required]
		public virtual string cReportConfig { get; set; }
		

    }
}