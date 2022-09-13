using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Divisions
{
	[Table("tblDivision")]
    public class Division : Entity 
    {

		[Required]
		[StringLength(DivisionConsts.MaxcDivisionNameLength, MinimumLength = DivisionConsts.MincDivisionNameLength)]
		public virtual string cDivisionName { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcDivisionDescriptionLength, MinimumLength = DivisionConsts.MincDivisionDescriptionLength)]
		public virtual string cDivisionDescription { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcAddress1Length, MinimumLength = DivisionConsts.MincAddress1Length)]
		public virtual string cAddress1 { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcAddress2Length, MinimumLength = DivisionConsts.MincAddress2Length)]
		public virtual string cAddress2 { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcCityLength, MinimumLength = DivisionConsts.MincCityLength)]
		public virtual string cCity { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcStateLength, MinimumLength = DivisionConsts.MincStateLength)]
		public virtual string cState { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcZipLength, MinimumLength = DivisionConsts.MincZipLength)]
		public virtual string cZip { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcPhoneLength, MinimumLength = DivisionConsts.MincPhoneLength)]
		public virtual string cPhone { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcFaxLength, MinimumLength = DivisionConsts.MincFaxLength)]
		public virtual string cFax { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcEmailLength, MinimumLength = DivisionConsts.MincEmailLength)]
		public virtual string cEmail { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcLogoPathLength, MinimumLength = DivisionConsts.MincLogoPathLength)]
		public virtual string cLogoPath { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcOfferPathLength, MinimumLength = DivisionConsts.MincOfferPathLength)]
		public virtual string cOfferPath { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcStatusLength, MinimumLength = DivisionConsts.MincStatusLength)]
		public virtual string cStatus { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		[StringLength(DivisionConsts.MaxcCreatedByLength, MinimumLength = DivisionConsts.MincCreatedByLength)]
		public virtual string cCreatedBy { get; set; }
		
		[StringLength(DivisionConsts.MaxcMNodifiedByLength, MinimumLength = DivisionConsts.MincMNodifiedByLength)]
		public virtual string cModifiedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		

    }
}