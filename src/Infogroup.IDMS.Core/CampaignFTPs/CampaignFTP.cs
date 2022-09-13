using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.CampaignFTPs
{
	[Table("tblOrderFTP")]
    public class CampaignFTP : Entity 
    {

		public virtual int OrderID { get; set; }
		
		[Required]
		public virtual string cFTPServer { get; set; }
		
		[Required]
		public virtual string cUserID { get; set; }
		
		[Required]
		public virtual string cPassword { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

    }
}