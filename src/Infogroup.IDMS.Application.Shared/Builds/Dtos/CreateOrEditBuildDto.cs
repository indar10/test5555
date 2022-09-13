
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Builds.Dtos
{
    public class CreateOrEditBuildDto : EntityDto<int?>
    {

		[StringLength(BuildConsts.MaxLK_BuildStatusLength, MinimumLength = BuildConsts.MinLK_BuildStatusLength)]
		public string LK_BuildStatus { get; set; }
		
		
		public int? iPreviousBuildID { get; set; }
		
		
		[Required]
		[StringLength(BuildConsts.MaxcBuildLength, MinimumLength = BuildConsts.MincBuildLength)]
		public string cBuild { get; set; }
		
		
		[Required]
		[StringLength(BuildConsts.MaxcDescriptionLength, MinimumLength = BuildConsts.MincDescriptionLength)]
		public string cDescription { get; set; }
		
		
		public DateTime? dMailDate { get; set; }
		
		
		public int iRecordCount { get; set; }
		
		
		public bool iIsReadyToUse { get; set; }
		
		
		public bool iIsOnDisk { get; set; }
		
		
		[StringLength(BuildConsts.MaxcMailDateFROMLength, MinimumLength = BuildConsts.MincMailDateFROMLength)]
		public string cMailDateFROM { get; set; }
		
		
		[StringLength(BuildConsts.MaxcMailDateTOLength, MinimumLength = BuildConsts.MincMailDateTOLength)]
		public string cMailDateTO { get; set; }
		
		
		public DateTime dCreatedDate { get; set; }
		
		
		[Required]
		[StringLength(BuildConsts.MaxcCreatedByLength, MinimumLength = BuildConsts.MincCreatedByLength)]
		public string cCreatedBy { get; set; }
		
		
		public DateTime? dModifiedDate { get; set; }
		
		
		[StringLength(BuildConsts.MaxcModifiedByLength, MinimumLength = BuildConsts.MincModifiedByLength)]
		public string cModifiedBy { get; set; }
		
		
		public int LK_BuildPriority { get; set; }
		
		
		public DateTime? dScheduledDateTime { get; set; }
		
		
		public bool iStopRequested { get; set; }
		
		
		public bool iIsOneStep { get; set; }
		
		
		 public int? DatabaseId { get; set; }
		 
		 
    }
}