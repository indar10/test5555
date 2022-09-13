using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Lookups.Dtos
{
    public class GetAllLookupsForExcelInput
    {
		public string Filter { get; set; }

		public int? MaxIDFilter { get; set; }
		public int? MinIDFilter { get; set; }

		public string cLookupValueFilter { get; set; }

		public int? MaxiOrderByFilter { get; set; }
		public int? MiniOrderByFilter { get; set; }

		public string cCodeFilter { get; set; }

		public string cDescriptionFilter { get; set; }

		public string cFieldFilter { get; set; }

		public string mFieldFilter { get; set; }

		public int? MaxiFieldFilter { get; set; }
		public int? MiniFieldFilter { get; set; }

		public int iIsActiveFilter { get; set; }

		public string cCreatedByFilter { get; set; }

		public string cModifiedByFilter { get; set; }

		public DateTime? MaxdCreatedDateFilter { get; set; }
		public DateTime? MindCreatedDateFilter { get; set; }

		public DateTime? MaxdModifiedDateFilter { get; set; }
		public DateTime? MindModifiedDateFilter { get; set; }



    }
}