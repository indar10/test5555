using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.Divisions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.UserDivisions
{
	[Table("tblUserDivision")]
    public class UserDivision : Entity 
    {

		public virtual int iSelectedBuildID { get; set; }
		
		public virtual int iSelectedDatabaseID { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

		public virtual int UserID { get; set; }
		
        [ForeignKey("UserID")]
		public IDMSUser UserFk { get; set; }
		
		public virtual int DivisionID { get; set; }
		
        [ForeignKey("DivisionID")]
		public Division DivisionFk { get; set; }
		
    }
}