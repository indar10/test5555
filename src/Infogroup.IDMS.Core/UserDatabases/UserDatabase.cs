using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Databases;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.UserDatabases
{
	[Table("tblUserDatabase")]
    public class UserDatabase : Entity 
    {

		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

		public virtual long UserId { get; set; }
		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }
		
		public virtual int DatabaseId { get; set; }
		
        [ForeignKey("DatabaseId")]
		public Database DatabaseFk { get; set; }
		
    }
}