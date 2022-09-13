using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ProcessQueues.Dtos
{
    public class GetAllProcessQueuesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string cQueueNameFilter { get; set; }

		public string cDescriptionFilter { get; set; }

		public int? MaxiAllowedThreadCountFilter { get; set; }
		public int? MiniAllowedThreadCountFilter { get; set; }

		public string LK_QueueTypeFilter { get; set; }

		public string LK_ProcessTypeFilter { get; set; }

		public int iIsSuspendedFilter { get; set; }

		public DateTime? MaxdCreatedDateFilter { get; set; }
		public DateTime? MindCreatedDateFilter { get; set; }

		public string cCreatedByFilter { get; set; }

		public DateTime? MaxdModifiedDateFilter { get; set; }
		public DateTime? MindModifiedDateFilter { get; set; }

		public string cModifiedByFilter { get; set; }



    }
}