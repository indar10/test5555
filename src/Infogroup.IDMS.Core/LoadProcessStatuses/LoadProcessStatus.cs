using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.LoadProcessStatuses
{
	[Table("tblLoadProcessStatus")]
    public class LoadProcessStatus : Entity 
    {

		public virtual int BuildLoLID { get; set; }
		
		public virtual int? iStatus { get; set; }
		
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dCreatedDate { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

    }
}