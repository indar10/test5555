using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.ListLoadStatuses
{
	[Table("tblListLoadStatus")]
    public class ListLoadStatus : Entity 
    {

		public virtual int BuildLoLID { get; set; }
		
		[Required]
		public virtual string LK_LoadStatus { get; set; }
		
		public virtual bool iIsCurrent { get; set; }
		
		[Required]
		public virtual string cCalculation { get; set; }
		
		public virtual string cNotes { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

    }
}