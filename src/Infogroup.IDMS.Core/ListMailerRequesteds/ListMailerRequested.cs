using Infogroup.IDMS.MasterLoLs;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ListMailerRequesteds
{
	[Table("tblListMailerRequested")]
    public class ListMailerRequested : Entity 
    {

		public virtual int MailerID { get; set; }
		
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

		public virtual int ListID { get; set; }
		
        [ForeignKey("ListID")]
		public MasterLoL MasterLoLFk { get; set; }
		
    }
}