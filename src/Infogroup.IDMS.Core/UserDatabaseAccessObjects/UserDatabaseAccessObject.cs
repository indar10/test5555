using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.AccessObjects;
using Infogroup.IDMS.Databases;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.UserDatabaseAccessObjects
{
	[Table("tblUserDatabaseAccessObject")]
    public class UserDatabaseAccessObject : Entity 
    {

		public virtual bool iListAccess { get; set; }
		
		public virtual bool iAddEditAccess { get; set; }
        public virtual int UserID { get; set; }
        [Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

		public virtual int IDMSUserId { get; set; }
		
        [ForeignKey("IDMSUserId")]
		public IDMSUser IDMSUserFk { get; set; }
		
		public virtual int AccessObjectId { get; set; }
		
        [ForeignKey("AccessObjectId")]
		public AccessObject AccessObjectFk { get; set; }
		
		public virtual int DatabaseId { get; set; }
		
        [ForeignKey("DatabaseId")]
		public Database DatabaseFk { get; set; }
		
    }
}