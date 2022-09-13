using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.IDMSTasks
{
	[Table("tblTask")]
    public class IDMSTask : Entity 
    {

		[Required]
		public virtual string cTaskName { get; set; }
		
		[Required]
		public virtual string cTaskDescription { get; set; }
		
		[Required]
		public virtual string cTaskUrl { get; set; }
		
		[Required]
		public virtual string cStatus { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

    }
}