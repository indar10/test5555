using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.MatchAppendStatuses
{
	[Table("tblMatchAppendStatus")]
    public class MatchAppendStatus : Entity 
    {

		public virtual int MatchAppendID { get; set; }
		
		public virtual int iStatusID { get; set; }
		
		public virtual bool iIsCurrent { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		

    }
}