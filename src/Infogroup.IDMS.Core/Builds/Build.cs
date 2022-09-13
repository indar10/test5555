using Infogroup.IDMS.Databases;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.Builds
{
	[Table("tblBuild")]
    public class Build : Entity 
    {

		[StringLength(BuildConsts.MaxLK_BuildStatusLength, MinimumLength = BuildConsts.MinLK_BuildStatusLength)]
		public virtual string LK_BuildStatus { get; set; }
		
		public virtual int? iPreviousBuildID { get; set; }
		
		[Required]
		[StringLength(BuildConsts.MaxcBuildLength, MinimumLength = BuildConsts.MincBuildLength)]
		public virtual string cBuild { get; set; }
		
		[Required]
		[StringLength(BuildConsts.MaxcDescriptionLength, MinimumLength = BuildConsts.MincDescriptionLength)]
		public virtual string cDescription { get; set; }
		
		public virtual DateTime? dMailDate { get; set; }
		
		public virtual int iRecordCount { get; set; }
		
		public virtual bool iIsReadyToUse { get; set; }
		
		public virtual bool iIsOnDisk { get; set; }
		
		[StringLength(BuildConsts.MaxcMailDateFROMLength, MinimumLength = BuildConsts.MincMailDateFROMLength)]
		public virtual string cMailDateFROM { get; set; }
		
		[StringLength(BuildConsts.MaxcMailDateTOLength, MinimumLength = BuildConsts.MincMailDateTOLength)]
		public virtual string cMailDateTO { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		[StringLength(BuildConsts.MaxcCreatedByLength, MinimumLength = BuildConsts.MincCreatedByLength)]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		[StringLength(BuildConsts.MaxcModifiedByLength, MinimumLength = BuildConsts.MincModifiedByLength)]
		public virtual string cModifiedBy { get; set; }
		
		public virtual int LK_BuildPriority { get; set; }
		
		public virtual DateTime? dScheduledDateTime { get; set; }
		
		public virtual bool iStopRequested { get; set; }
		
		public virtual bool iIsOneStep { get; set; }
		

		public virtual int? DatabaseId { get; set; }
		
        [ForeignKey("DatabaseId")]
		public Database Database { get; set; }
		
    }
}