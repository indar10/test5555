using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class GetAllSegmentsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

        public string cDescriptionFilter { get; set; }

        public int iUseAutosuppressFilter { get; set; }

        public string cKeyCode1Filter { get; set; }

        public string cKeyCode2Filter { get; set; }

        public int? MaxiRequiredQtyFilter { get; set; }
        public int? MiniRequiredQtyFilter { get; set; }

        public int? MaxiProvidedQtyFilter { get; set; }
        public int? MiniProvidedQtyFilter { get; set; }

        public int? MaxiDedupeOrderSpecifiedFilter { get; set; }
        public int? MiniDedupeOrderSpecifiedFilter { get; set; }

        public int? MaxiDedupeOrderUsedFilter { get; set; }
        public int? MiniDedupeOrderUsedFilter { get; set; }

        public string cMaxPerGroupFilter { get; set; }

        public string cTitleSuppressionFilter { get; set; }

        public string cFixedTitleFilter { get; set; }

        public int? MaxiDoubleMultiBuyerCountFilter { get; set; }
        public int? MiniDoubleMultiBuyerCountFilter { get; set; }

        public int iIsOrderLevelFilter { get; set; }

        public DateTime? MaxdCreatedDateFilter { get; set; }
        public DateTime? MindCreatedDateFilter { get; set; }

        public string cCreatedByFilter { get; set; }

        public DateTime? MaxdModifiedDateFilter { get; set; }
        public DateTime? MindModifiedDateFilter { get; set; }

        public string cModifiedByFilter { get; set; }

        public int? MaxiGroupFilter { get; set; }
        public int? MiniGroupFilter { get; set; }

        public int? MaxiOutputQtyFilter { get; set; }
        public int? MiniOutputQtyFilter { get; set; }

        public int? MaxiAvailableQtyFilter { get; set; }
        public int? MiniAvailableQtyFilter { get; set; }

        public int iIsRandomRadiusNthFilter { get; set; }

        public string OrdercDatabaseNameFilter { get; set; }


    }
}