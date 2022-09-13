using Abp.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Infogroup.IDMS.Segments.Dtos;
using System.Linq.Dynamic.Core;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Validation;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.SegmentSelections.Dtos;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Infogroup.IDMS.Common;
using Abp.Extensions;

namespace Infogroup.IDMS.SegmentSelections
{
    [AbpAuthorize]
    public partial class SegmentSelectionsAppService : IDMSAppServiceBase, ISegmentSelectionsAppService
    {
        private const string OutputQuantityField = "iOutputQty";
        private const string DescriptionField = "cDescription";
        public async Task<GlobalChangesDto> GetSegmentsForGlobalChanges(GetSegmentListInput input)
        {
            try
            {
                var noOfSegmentsforOrder = _segmentRepository.GetAll().Count(x => x.OrderId == input.OrderId);
                var finalFilterText = input.Filter;
                if (input.Filter.Contains(','))
                    input.Filter = CommonHelpers.GetSplitCommaSeparatedString(input.Filter, noOfSegmentsforOrder, true);
                if (string.IsNullOrWhiteSpace(input.Filter))
                    throw new UserFriendlyException(L("RangeValidation"));
                var query = GetSegmentsForGlobalChangesQuery(input);
                var pagedSegments = await _customSegmentRepository.GetAllSegmentsForGlobalChanges(query);
                return new GlobalChangesDto
                {
                    FinalFilterText = finalFilterText,
                    PagedSegments = pagedSegments
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<List<DropdownOutputDto>> GetFieldsForFindReplace(int buildId, int databaseId , int mailerId)
        {
            try
            {
                return await _customBuildTableLayoutRepository.GetFindReplaceFields(buildId, databaseId, mailerId);
            }

            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        private async Task<string> ApplyFindReplace(SaveGlobalChangesInputDto input)
        {
            try
            {
                int modifiedRulesCount;
                var segmentSelectionSaveDto = new SegmentSelectionSaveDto
                {
                    campaignId = input.CampaignId,
                    DatabaseId = input.DatabaseId,
                    deletedSelections = new List<int>()
                };
                var segmentIds = await GetSegmentIdsForGlobalChanges(input.FilterText, input.CampaignId);
                if (!string.IsNullOrEmpty(input.FilterText)) input.FilterText = input.FilterText.Trim();
                var fileExtensions = new List<string> { "%.csv", "%.txt" };
                var selectionQuery = _segmentSelectionRepository.GetAll()
                                                   .Where(selection => selection.cValues.Contains(input.SearchValue))
                                                   .Where(selection => segmentIds.Contains(selection.SegmentId));

                if (input.FieldDescription == "Zip Radius")
                {
                    var zipRadiusSelectionsDtos = selectionQuery.Where(selection => selection.cQuestionDescription == input.FieldDescription)
                                             .Where((selection => !fileExtensions.Any(ext => selection.cValues.Contains(ext))))
                                             .Select(selection => ObjectMapper.Map<SegmentSelectionDto>(selection))
                                             .ToList();
                    modifiedRulesCount = zipRadiusSelectionsDtos.Count;
                    foreach (var selectionDto in zipRadiusSelectionsDtos)
                    {
                        selectionDto.cValues = Regex.Replace(selectionDto.cValues, input.SearchValue, input.ReplaceValue, RegexOptions.IgnoreCase)
                            .Replace(input.SearchValue, input.ReplaceValue)
                            .Replace("'", "''")
                            .Replace("\r\n", "\n");
                        selectionDto.cValueMode = "T";
                        selectionDto.isRuleUpdated = 1;
                    }
                    segmentSelectionSaveDto.selections = zipRadiusSelectionsDtos;
                }
                else
                {
                    var selectionsDtos = selectionQuery.Where(selection => selection.cQuestionFieldName == input.FieldName)
                                            .Where(selection => selection.cValueMode == "T" || selection.cValueMode == "G")
                                            .Select(selection => ObjectMapper.Map<SegmentSelectionDto>(selection))
                                            .ToList();
                    modifiedRulesCount = selectionsDtos.Count;
                    foreach (var selectionDto in selectionsDtos)
                    {
                        selectionDto.isRuleUpdated = 1;
                        if (selectionDto.cValueMode.Equals("T"))
                        {
                            selectionDto.cValues = Regex.Replace(selectionDto.cValues, input.SearchValue, input.ReplaceValue, RegexOptions.IgnoreCase)
                                    .Replace("'", "''")
                                    .Replace("\r\n", "\n");
                        }
                        else
                        {
                            var valueDescriptions = await _customBuildTableLayoutRepository.GetValues(input.FieldId);
                            var updatedValuesText = Regex.Replace(selectionDto.cValues, input.SearchValue, input.ReplaceValue, RegexOptions.IgnoreCase).Replace("'", "''").Replace("\r\n", "\n");
                            var updatedValues = updatedValuesText.Split(",").ToList();
                            var updatedDescriptions = new List<string>();
                            foreach (var value in updatedValues)
                            {
                                var description = valueDescriptions.Where(t => t.Value.ToString().Trim().ToLower() == value.Trim().ToLower()).FirstOrDefault()?.Label ?? string.Empty;
                                updatedDescriptions.Add(description);
                            }
                            selectionDto.cValues = updatedValuesText;
                            selectionDto.cDescriptions = string.Join(",", updatedDescriptions);
                        }
                        segmentSelectionSaveDto.selections = selectionsDtos;
                    }
                }
                if (modifiedRulesCount > 0)
                    await CreateSegmentSelectionDetails(segmentSelectionSaveDto);
                else return L("NoSelectionsFound", input.SearchValue);
                return $"{modifiedRulesCount} rules updated.";
            }

            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public async Task<string> SaveGlobalChanges(SaveGlobalChangesInputDto input)
        {
            try
            {
                var result = "";
                if (input.ReplaceValue == null)
                    input.ReplaceValue = string.Empty;
                var newStatus = CampaignStatus.OrderCreated;
                if (input.Action == GlobalChangesAction.FIND_REPLACE)
                {
                    result = await ApplyFindReplace(input);
                }
                else
                {
                    input.UserID = _mySession.IDMSUserName;
                    var segments = await GetSegmentIdsForGlobalChanges(input.FilterText, input.CampaignId);
                    input.TargetSegments = string.Join(",", segments);
                    if (input.Action == GlobalChangesAction.CAMPAIGN_HISTORY)
                        result = ProcessCampaignHistory(input);
                    else
                        result = await _customSegmentSelectionRepository.ApplyBulkOperations(input);
                }
                if (!(input.Action == GlobalChangesAction.EDIT_SEGMENTS && input.FieldName == DescriptionField))
                {
                    if (input.Action == GlobalChangesAction.EDIT_SEGMENTS && input.FieldName == OutputQuantityField)
                    {
                        switch (input.campaignStatus)
                        {
                            case CampaignStatus.OutputFailed:
                            case CampaignStatus.OutputCompleted:
                            case CampaignStatus.OrderCompleted:
                                newStatus = CampaignStatus.OrderCompleted;
                                break;
                            default:
                                newStatus = CampaignStatus.OrderCreated;
                                break;
                        }
                    }
                    else if (input.Action != GlobalChangesAction.FIND_REPLACE)
                        newStatus = CampaignStatus.OrderCreated;
                    await _orderStatusManager.UpdateOrderStatus(input.CampaignId, newStatus, _mySession.IDMSUserName);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        private string ProcessCampaignHistory(SaveGlobalChangesInputDto input)
        {
            var inputIDs = input.SearchValue.Trim(',').Split(',')
                    .Select(code => code.Trim())
                    .Where(code => !string.IsNullOrEmpty(code))
                    .Distinct()
                    .ToList();
            var commaSeparatedIds = string.Join(",", inputIDs);
            var query = GetValidPreviousCampaigns(commaSeparatedIds, input.DatabaseId, input.DivisionId, input.CampaignId);
            var validCampaignId = _campaignHistoryRepository.GetValidPreviousCampaigns(query);
            var invalidCampaignIds = inputIDs.Except(validCampaignId).ToArray();
            if (invalidCampaignIds.IsNullOrEmpty())
            {
                input.SearchValue = commaSeparatedIds;
                return _campaignHistoryRepository.BulkOperationOnCampaignHistory(input, _mySession.IDMSUserName);
            }
            else
            {
                var dynamicMessage = invalidCampaignIds.Length > 1 ? $"s {string.Join(", ", invalidCampaignIds, 0, invalidCampaignIds.Length - 1)} and {invalidCampaignIds.LastOrDefault()} are" :
                    invalidCampaignIds.Length == 1 ? $" {invalidCampaignIds.FirstOrDefault()} is" : string.Empty;
                throw new UserFriendlyException(L("InvalidCampaignIdError", dynamicMessage));
            }
        }


        private Tuple<string, string, List<SqlParameter>> GetSegmentsForGlobalChangesQuery(GetSegmentListInput input, SegmentSelectionType selectionType = SegmentSelectionType.BulkOperations)
        {
            try
            {
                if (!string.IsNullOrEmpty(input.Filter)) input.Filter = input.Filter.Trim();
                List<string> dedupeOrderValues;
                var query = new QueryBuilder();
                var columns = "";
                switch (selectionType)
                {
                    case SegmentSelectionType.BulkOperations:
                        columns = "S.ID,S.iDedupeOrderSpecified,S.cDescription,S.iRequiredQty,S.iProvidedQty,S.iAvailableQty,S.cKeyCode1";
                        break;
                    case SegmentSelectionType.InlineEdit:
                        columns = "S.*";
                        break;
                    case SegmentSelectionType.ID:
                        columns = "S.ID";
                        break;
                    default:
                        break;
                }
                query.AddSelect(columns);
                query.AddFrom("tblSegment", "S");
                query.AddNoLock();
                query.AddWhere("AND", "S.OrderID", "IN", input.OrderId.ToString());
                query.AddWhere("AND", "S.iIsOrderLevel", "EQUALTO", "0");
                if (input.Filter.Contains(":"))
                {
                    query.AddWhereString($"AND { _shortSearch.GetWhere(PageID.AddFavorites, input.Filter)}{Environment.NewLine}");
                }
                else if (ValidationHelper.IsNumeric(input.Filter))
                {
                    query.AddWhere("AND", "S.iDedupeOrderSpecified", "IN", input.Filter.Split(','));
                }
                else if (ValidationHelper.IsNumericRange(input.Filter))
                {
                    dedupeOrderValues = input.Filter.Split('-').ToList();
                    query.AddWhere("AND", "S.iDedupeOrderSpecified", "GREATERTHAN_OR_EQUALTO", dedupeOrderValues[0]);
                    query.AddWhere("AND", "S.iDedupeOrderSpecified", "LESSTHAN_OR_EQUALTO", dedupeOrderValues[1]);
                }
                else
                {
                    query.AddWhere("AND", "S.cDescription", "LIKE", input.Filter);
                }
                query.AddDistinct();
                if (selectionType == SegmentSelectionType.BulkOperations || selectionType == SegmentSelectionType.InlineEdit)
                {
                    query.AddSort(input.Sorting != null ? $"S.{input.Sorting}" : "S.iDedupeOrderSpecified ASC");
                }
                if (selectionType == SegmentSelectionType.BulkOperations)
                    query.AddOffset($"OFFSET {input.SkipCount} ROWS FETCH NEXT 10 ROWS ONLY;");

                (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
                var sqlCount = selectionType != SegmentSelectionType.BulkOperations ? string.Empty : query.BuildCount().Item1;
                return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private async Task<List<int>> GetSegmentIdsForGlobalChanges(string filter, int campaignId)
        {
            var noOfSegmentsforOrder = _segmentRepository.GetAll().Count(x => x.OrderId == campaignId);

            if (filter.Contains(','))
                filter = CommonHelpers.GetSplitCommaSeparatedString(filter, noOfSegmentsforOrder);
            if (string.IsNullOrWhiteSpace(filter))
                throw new UserFriendlyException(L("RangeValidation"));
            var segmentInput = new GetSegmentListInput
            {
                Filter = filter,
                OrderId = campaignId
            };
            var query = GetSegmentsForGlobalChangesQuery(segmentInput, SegmentSelectionType.ID);
            return await _customSegmentRepository.GetAllSegmentIDsForGlobalChanges(query);
        }
        private Tuple<string, List<SqlParameter>> GetValidPreviousCampaigns(string inputCampaignIds, int databaseId, int divisionId, int campaignId)
        {
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@InputIds", inputCampaignIds));
            var configCValue = _idmsConfigurationCache.GetConfigurationValue("UseDivisionalCustomer").cValue;
            var divId = 0;
            var isDivisionalDatabase = configCValue.Split(',')
                       .Select(id => { int.TryParse(id, out divId); return divId; })
                       .Any(id => id == divisionId);
            var whereClause = isDivisionalDatabase ? $"D.DivisionID = {divisionId}" : $"M.DatabaseID = {databaseId}";
            var joinClause = isDivisionalDatabase ? "INNER JOIN tblDatabase D WITH (NOLOCK) ON D.ID = M.DatabaseID" : string.Empty;
            var sqlQuery = $@"SELECT DISTINCT O.ID
                            FROM tblorder O WITH (NOLOCK)
                            INNER JOIN tblOrderStatus OS WITH (NOLOCK) ON OS.OrderID = O.ID
                            AND OS.iIsCurrent = 1
                            AND OS.iStatus > = 40
                            AND OS.iStatus NOT IN (150,100,50)
                            AND OS.dCreatedDate >= DATEADD(DAY,DATEDIFF(DAY,0,GETDATE())-400,0) 
                            INNER JOIN tblMailer M WITH (NOLOCK) ON O.MailerID = M.ID
                            INNER JOIN tblOffer F WITH (NOLOCK) ON O.OfferID = F.ID
                            {joinClause}
                            INNER JOIN tblGroupBroker GB WITH (NOLOCK) on M.BrokerID = GB.BrokerID
                            INNER JOIN tblUserGroup UG WITH (NOLOCK) on UG.GroupID = GB.GroupID
                            AND O.ID IN (SELECT CAST(VALUE AS INT) FROM STRING_SPLIT(@InputIds, ','))
                            AND O.ID <> {campaignId} 
                            AND UG.UserID= {_mySession.IDMSUserId}
                            AND {whereClause}
                            ORDER BY O.ID ASC;";

            return new Tuple<string, List<SqlParameter>>(sqlQuery, sqlParams);
        }
    }
}
