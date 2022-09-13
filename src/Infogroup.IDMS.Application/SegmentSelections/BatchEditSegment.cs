
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Authorization;
using Infogroup.IDMS.Segments.Dtos;
using System.Linq.Dynamic.Core;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Segments;
using System.Threading.Tasks;
using Infogroup.IDMS.OrderStatuss;

namespace Infogroup.IDMS.SegmentSelections
{
    [AbpAuthorize]
    public partial class SegmentSelectionsAppService : IDMSAppServiceBase, ISegmentSelectionsAppService
    {
        public List<BatchEditSegmentDto> GetSegmentsForInlineEdit(GetSegmentListInput input)
        {
            try
            {
                var noOfSegmentsforOrder = _segmentRepository.GetAll().Count(x => x.OrderId == input.OrderId);
                if (input.Filter.Contains(','))
                    input.Filter = CommonHelpers.GetSplitCommaSeparatedString(input.Filter, noOfSegmentsforOrder, true);
                if (string.IsNullOrWhiteSpace(input.Filter))
                    throw new UserFriendlyException(L("RangeValidation"));
                var query = GetSegmentsForGlobalChangesQuery(input, SegmentSelectionType.InlineEdit);
                return _customSegmentRepository.GetAllSegmentsForBatchEdit(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public GetInitialStateForBatchEdit GetInitialStateForBatchEdit(int buildId, int databaseId)
        {
            try
            {
                return new GetInitialStateForBatchEdit
                {
                    IsCalculateDistanceSet = _idmsConfigurationCache.GetConfigurationValue("CalculateDistance", databaseId).cValue == "0",
                    HasDefaultRules = _buildTableLayoutManager.ContainsAutoSupress(buildId, databaseId),
                    MaxPers = _lookupCache.GetLookUpFields("MAXPER").OrderBy(lookup => lookup.cCode).Select(seg => new DropdownOutputDto
                    {
                        Value = seg.cCode,
                        Label = seg.cDescription
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task SaveBatchSegments(SaveBatchSegmentDto input)
        {
            try
            {
                var modificationDate = DateTime.Now;
                foreach (var modifiedSegmentDto in input.ModifiedSegments)
                {
                    modifiedSegmentDto.dModifiedDate = modificationDate;
                    modifiedSegmentDto.cModifiedBy = _mySession.IDMSUserName;
                    var segment = _segmentRepository.FirstOrDefault((int)modifiedSegmentDto.Id);
                    modifiedSegmentDto.cNthPriorityField = segment.cNthPriorityField;
                    modifiedSegmentDto.cNthPriorityFieldOrder = segment.cNthPriorityFieldOrder;
                    ObjectMapper.Map(modifiedSegmentDto, segment);
                }
                CurrentUnitOfWork.SaveChanges();
                if (input.ModifiedSegments.Count > 0 && input.NextStatus < 50)
                    await _orderStatusManager.UpdateOrderStatus(input.ModifiedSegments[0].OrderId, (CampaignStatus)input.NextStatus, _mySession.IDMSUserName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
