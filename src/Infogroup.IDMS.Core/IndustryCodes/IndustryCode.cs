using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.IndustryCodes
{
	[Table("tblIndustryCode")]
    public class IndustryCode : Entity 
    {

		public virtual string cSICCode { get; set; }
		
		public virtual string cIndustrySpecificCode { get; set; }
		
		[Required]
		public virtual string cPositionIndicator { get; set; }
		
		public virtual string cIndustrySpecificDescription { get; set; }
		
		public virtual string cRangeFromValue { get; set; }
		
		public virtual string cRangeToValue { get; set; }
		

    }
}