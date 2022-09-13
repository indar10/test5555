using Infogroup.IDMS.IDMSUsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.UserGroups
{
	[Table("tblUserGroup")]
    public class UserGroup : Entity 
    {

		public virtual int GroupID { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

		public virtual int UserId { get; set; }
		
        [ForeignKey("UserId")]
		public IDMSUser UserFk { get; set; }
		
    }
}