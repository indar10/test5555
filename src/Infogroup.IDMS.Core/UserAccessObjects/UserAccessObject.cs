using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.AccessObjects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.UserAccessObjects
{
	[Table("tblUserAccessObject")]
    public class UserAccessObject : Entity 
    {

		public virtual bool iListAccess { get; set; }
		
		public virtual bool iAddEditAccess { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }

		public virtual int UserID { get; set; }

		public virtual int IDMSUserId { get; set; }
		
        [ForeignKey("IDMSUserId")]
		public IDMSUser IDMSUserFk { get; set; }
		
		public virtual int AccessObjectId { get; set; }
		
        [ForeignKey("AccessObjectId")]
		public AccessObject AccessObjectFk { get; set; }
		
    }
}