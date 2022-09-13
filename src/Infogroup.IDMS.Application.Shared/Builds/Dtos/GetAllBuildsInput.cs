using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Builds.Dtos
{
    public class GetAllBuildsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string LK_BuildStatusFilter { get; set; }

		public int? MaxiPreviousBuildIDFilter { get; set; }
		public int? MiniPreviousBuildIDFilter { get; set; }

		public string cBuildFilter { get; set; }

		public string cDescriptionFilter { get; set; }

		public DateTime? MaxdMailDateFilter { get; set; }
		public DateTime? MindMailDateFilter { get; set; }

		public int? MaxiRecordCountFilter { get; set; }
		public int? MiniRecordCountFilter { get; set; }

		public int iIsReadyToUseFilter { get; set; }

		public int iIsOnDiskFilter { get; set; }

		public string cMailDateFROMFilter { get; set; }

		public string cMailDateTOFilter { get; set; }

		public DateTime? MaxdCreatedDateFilter { get; set; }
		public DateTime? MindCreatedDateFilter { get; set; }

		public string cCreatedByFilter { get; set; }

		public DateTime? MaxdModifiedDateFilter { get; set; }
		public DateTime? MindModifiedDateFilter { get; set; }

		public string cModifiedByFilter { get; set; }

		public int? MaxLK_BuildPriorityFilter { get; set; }
		public int? MinLK_BuildPriorityFilter { get; set; }

		public DateTime? MaxdScheduledDateTimeFilter { get; set; }
		public DateTime? MindScheduledDateTimeFilter { get; set; }

		public int iStopRequestedFilter { get; set; }

		public int iIsOneStepFilter { get; set; }


		 public string DatabasecDatabaseNameFilter { get; set; }

		 
    }
}