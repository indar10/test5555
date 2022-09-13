using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.Reports;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.UserReports
{
	[Table("tblUserReport")]
    public class UserReport : Entity
    {
					
		public virtual int UserID { get; set; }
		
		//public virtual int ReportID { get; set; }
		

		//public virtual int? TblUserId { get; set; }
		
        [ForeignKey("UserID")]
		public IDMSUser IDMSUserFk { get; set; }
		
		public virtual int ReportID { get; set; }
		
        [ForeignKey("ReportID")]
		public Report ReportFk { get; set; }
		
    }
}