using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ProcessQueueDatabases.Dtos
{
    public class GetAllProcessQueueDatabasesForExcelInput
    {
		public string Filter { get; set; }

		public int? MaxDatabaseIdFilter { get; set; }
		public int? MinDatabaseIdFilter { get; set; }

		public string cCreatedByFilter { get; set; }

		public DateTime? MaxdCreatedDateFilter { get; set; }
		public DateTime? MindCreatedDateFilter { get; set; }

		public string cModifiedByFilter { get; set; }

		public DateTime? MaxdModifiedDateFilter { get; set; }
		public DateTime? MindModifiedDateFilter { get; set; }


		 public string ProcessQueuecQueueNameFilter { get; set; }

		 
    }
}