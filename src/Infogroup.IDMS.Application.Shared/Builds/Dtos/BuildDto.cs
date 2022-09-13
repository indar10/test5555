
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Builds.Dtos
{
    public class BuildDto : EntityDto
    {
		public string LK_BuildStatus { get; set; }

		public int? iPreviousBuildID { get; set; }

		public string cBuild { get; set; }

		public string cDescription { get; set; }

		public DateTime? dMailDate { get; set; }

		public int iRecordCount { get; set; }

		public bool iIsReadyToUse { get; set; }

		public bool iIsOnDisk { get; set; }

		public string cMailDateFROM { get; set; }

		public string cMailDateTO { get; set; }

		public DateTime dCreatedDate { get; set; }

		public string cCreatedBy { get; set; }

		public DateTime? dModifiedDate { get; set; }

		public string cModifiedBy { get; set; }

		public int LK_BuildPriority { get; set; }

		public DateTime? dScheduledDateTime { get; set; }

		public bool iStopRequested { get; set; }

		public bool iIsOneStep { get; set; }


		 public int? DatabaseId { get; set; }

		 
    }
}