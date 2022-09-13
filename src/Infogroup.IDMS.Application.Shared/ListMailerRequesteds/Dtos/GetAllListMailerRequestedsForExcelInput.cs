using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ListMailerRequesteds.Dtos
{
    public class GetAllListMailerRequestedsForExcelInput
    {
		public string Filter { get; set; }

		public int? MaxMailerIDFilter { get; set; }
		public int? MinMailerIDFilter { get; set; }

		public string cCreatedByFilter { get; set; }

		public DateTime? MaxdCreatedDateFilter { get; set; }
		public DateTime? MindCreatedDateFilter { get; set; }

		public string cModifiedByFilter { get; set; }

		public DateTime? MaxdModifiedDateFilter { get; set; }
		public DateTime? MindModifiedDateFilter { get; set; }


		 public string MasterLoLcListNameFilter { get; set; }

		 
    }
}