using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.SICCodeRelateds
{
	[Table("tblSICCodeRelated")]
    public class SICCodeRelated : Entity 
    {

		[Required]
		public virtual string cSICCode { get; set; }
		
		[Required]
		public virtual string cRelatedSICCode { get; set; }
		
		[Required]
		public virtual string cRelatedSICDescription { get; set; }
		
		public virtual string cIndicator { get; set; }
		

    }
}