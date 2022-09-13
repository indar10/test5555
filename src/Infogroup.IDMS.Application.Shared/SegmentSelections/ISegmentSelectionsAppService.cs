using Abp.Application.Services;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Segments.Dtos;
using Infogroup.IDMS.SegmentSelections.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SegmentSelections
{
    public interface ISegmentSelectionsAppService : IApplicationService
    {
        Task CreateSegmentSelectionDetails(SegmentSelectionSaveDto input, bool isFromSelection);
        Task DeleteAll(int segmentId, int campaignId);
        string GetLayoutNameFromCampaignId(int campaignId);
        int GetNewMaxGroupId(int segmentId);
        Task SaveMultiFieldSelection(SegmentSelectionSaveDto selections);
        GetQueryBuilderDetails GetSelectionFieldsNew(int id, string isSegment, int databaseId, int buildId, int mailerId);
        int GetSegmentIdForOrderLevel(int campaignId);
        Task<bool> SaveBulkSegmentFileData(BulkSegmentDto bulkSegmentData);
        FileDto DownloadTemplate(bool isDefaultFormat, int databaseId);
        Task AddDefaultSelections(int segmentId, int buildId, int databaseId, string channelType);
        List<DropdownOutputDto> GetOperators();
        List<DropdownOutputDto> GetSubSelectFieldValues(string fieldId, int iBuildLoLID);
        Task SaveBatchSegments(SaveBatchSegmentDto input);
        GetInitialStateForBatchEdit GetInitialStateForBatchEdit(int buildId, int databaseId);
        List<BatchEditSegmentDto> GetSegmentsForInlineEdit(GetSegmentListInput input);
    }
}
