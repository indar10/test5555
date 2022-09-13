using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ListMailers.Dtos
{
    public class GetAllListMailersForExcelInput
    {
		public string Filter { get; set; }

		public int? MaxIDFilter { get; set; }
		public int? MinIDFilter { get; set; }

		public int? MaxMailerIDFilter { get; set; }
		public int? MinMailerIDFilter { get; set; }

		public DateTime? MaxdCreatedDateFilter { get; set; }
		public DateTime? MindCreatedDateFilter { get; set; }

		public string cCreatedByFilter { get; set; }

		public string cModifiedByFilter { get; set; }

		public DateTime? MaxdModifiedDateFilter { get; set; }
		public DateTime? MindModifiedDateFilter { get; set; }


		 public string MasterLoLcListNameFilter { get; set; }

		 
    }
}