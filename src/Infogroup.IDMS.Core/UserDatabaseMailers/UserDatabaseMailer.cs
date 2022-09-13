using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.Databases;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.UserDatabaseMailers
{
	[Table("tblUserDatabaseMailer")]
    public class UserDatabaseMailer : Entity 
    {

		public virtual int MailerID { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

		public virtual int UserId { get; set; }
		
        [ForeignKey("UserId")]
		public IDMSUser UserFk { get; set; }
		
		public virtual int DatabaseId { get; set; }
		
        [ForeignKey("DatabaseId")]
		public Database DatabaseFk { get; set; }
		
    }
}