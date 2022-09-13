using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.BuildLoLs.Dtos
{
    public class GetAllBuildLolsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public int? MaxMasterLolIDFilter { get; set; }
		public int? MinMasterLolIDFilter { get; set; }

		public string LK_ActionFilter { get; set; }

		public string LK_ActionMonth1Filter { get; set; }

		public string LK_ActionMonth2Filter { get; set; }

		public string LK_ActionNextMonthFilter { get; set; }

		public string LK_QuantityTypeFilter { get; set; }

		public string LK_FileTypeFilter { get; set; }

		public int iSkipFirstRowFilter { get; set; }

		public int iIsActiveFilter { get; set; }

		public int? MaxiUsageFilter { get; set; }
		public int? MiniUsageFilter { get; set; }

		public int? MaxnTurnsFilter { get; set; }
		public int? MinnTurnsFilter { get; set; }

		public string cDecisionReasoningFilter { get; set; }

		public string cSlugDateFilter { get; set; }

		public string cBatchDateTypeFilter { get; set; }

		public string LK_SlugDateTypeFilter { get; set; }

		public int? MaxiQuantityPreviousFilter { get; set; }
		public int? MiniQuantityPreviousFilter { get; set; }

		public int? MaxiQuantityRequestedFilter { get; set; }
		public int? MiniQuantityRequestedFilter { get; set; }

		public int? MaxiQuantityReceivedDPFilter { get; set; }
		public int? MiniQuantityReceivedDPFilter { get; set; }

		public int? MaxiQuantityReceivedFilter { get; set; }
		public int? MiniQuantityReceivedFilter { get; set; }

		public int? MaxiQuantityConvertedFilter { get; set; }
		public int? MiniQuantityConvertedFilter { get; set; }

		public DateTime? MaxdDateReceivedFilter { get; set; }
		public DateTime? MindDateReceivedFilter { get; set; }

		public int? MaxiQuantityTotalFilter { get; set; }
		public int? MiniQuantityTotalFilter { get; set; }

		public string cBatch_LastFROMFilter { get; set; }

		public string cBatch_LastTOFilter { get; set; }

		public string cBatch_FROMFilter { get; set; }

		public string cBatch_TOFilter { get; set; }

		public string Order_NoFilter { get; set; }

		public string Order_ClientPOFilter { get; set; }

		public string OrderSelectionFilter { get; set; }

		public string Order_FieldsFilter { get; set; }

		public string Order_CommentsFilter { get; set; }

		public string Order_Notes1Filter { get; set; }

		public string Order_Notes2Filter { get; set; }

		public string LK_EmailTemplateFilter { get; set; }

		public DateTime? MaxddateOrderSentFilter { get; set; }
		public DateTime? MinddateOrderSentFilter { get; set; }

		public string cNoteFilter { get; set; }

		public int? MaxiCASApprovalToFilter { get; set; }
		public int? MiniCASApprovalToFilter { get; set; }

		public string cSourceFilenameReadyToLoadFilter { get; set; }

		public string cSystemFilenameReadyToLoadFilter { get; set; }

		public string LK_LoadFileTypeFilter { get; set; }

		public string LK_LoadFileRowTerminatorFilter { get; set; }

		public string cOnePassFileNameFilter { get; set; }

		public DateTime? MaxdCreatedDateFilter { get; set; }
		public DateTime? MindCreatedDateFilter { get; set; }

		public string cCreatedByFilter { get; set; }

		public DateTime? MaxdModifiedDateFilter { get; set; }
		public DateTime? MindModifiedDateFilter { get; set; }

		public string cModifiedByFilter { get; set; }

		public string cSQLFilter { get; set; }

		public string cSQLDescriptionFilter { get; set; }

		public int? MaxiLoadQtyFilter { get; set; }
		public int? MiniLoadQtyFilter { get; set; }

		public string LK_EncodingFilter { get; set; }

		public int iIsMultilineFilter { get; set; }


		 public string BuildLK_BuildStatusFilter { get; set; }

		 
    }
}