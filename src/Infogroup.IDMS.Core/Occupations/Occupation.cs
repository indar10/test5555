using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Occupations
{
	[Table("tblOccupation")]
    public class Occupation : Entity 
    {

		[Required]
		public virtual string cIndustryCode { get; set; }
		
		[Required]
		public virtual string cIndustry { get; set; }
		
		[Required]
		public virtual string cOccupationCode { get; set; }
		
		[Required]
		public virtual string cOccupationtitle { get; set; }
		
		[Required]
		public virtual string cSpecialityCode { get; set; }
		
		[Required]
		public virtual string cSpecialtytitle { get; set; }
		
		public virtual int Count { get; set; }
		

    }
}