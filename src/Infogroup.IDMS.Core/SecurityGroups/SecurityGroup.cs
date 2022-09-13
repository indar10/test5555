using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.SecurityGroups
{
	[Table("tblGroup")]
    public class SecurityGroup : Entity 
    {

		public virtual int DatabaseID { get; set; }
		
		[Required]
		public virtual string cGroupName { get; set; }
		
		[Required]
		public virtual string cGroupDescription { get; set; }
		
		[Required]
		public virtual string cStatus { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual bool? iIsActive { get; set; }
		

    }
}