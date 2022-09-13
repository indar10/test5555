using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.MatchAppendDatabaseUsers
{
	[Table("tblMatchAppendDatabaseUser")]
    public class MatchAppendDatabaseUser : Entity 
    {

		public virtual int UserID { get; set; }
		
		public virtual int DatabaseID { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime cCreatedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

    }
}