using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.AutoSuppresses;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.CampaignExportLayouts;
using Infogroup.IDMS.CampaignMaxPers;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.Configuration;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.ExternalBuildTableDatabases;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.Segments.Dtos;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Shared.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Infogroup.IDMS.SegmentSelections;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Castle.DynamicLinqQueryBuilder;

namespace Infogroup.IDMS.Segments
{
    [AbpAuthorize(AppPermissions.Pages_Segments, AppPermissions.Pages_FastCount)]
    public partial class SegmentsAppService : IDMSAppServiceBase, ISegmentsAppService
    {
        private readonly IRepository<BuildTableLayout, int> _buildTableLayoutRepository;
        private readonly IRepository<Segment> _segmentRepository;
        private readonly IRepository<Lookup, int> _lookupRepository;
        private readonly ISegmentRepository _customSegmentRepository;
        private readonly IRepository<OrderStatus> _campaignStatusRepository;
        private readonly IRepository<CampaignMaxPer> _campaignMaxPerRepository;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly SegmentBizness _segmentBizness;
        private readonly AppSession _mySession;
        private readonly IOrderStatusManager _orderStatusManager;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IRepository<AutoSuppress, Guid> _autoSuppressRepository;
        private readonly IRepository<Build, int> _buildRepository;
        private readonly IRepository<BuildTable, int> _buildTableRepository;
        private readonly IRepository<Campaign, int> _campaignRepository;
        private readonly IRepository<ExternalBuildTableDatabase> _externalBuildTableDatabaseRepository;
        private readonly IRepository<CampaignExportLayout> _campaignExportLayoutRepository;
        private readonly IRedisLookupCache _lookupCache;
        private readonly ISegmentSelectionsAppService _segmentSelectionsAppService;
        private readonly string _webRootPath;


        public SegmentsAppService(IRepository<Segment> segmentRepository, ISegmentRepository customSegmentRepository,
            IRepository<OrderStatus> campaignStatusRepository, SegmentBizness segmentBizness, AppSession mySession,
            IOrderStatusManager orderStatusManager, IRepository<CampaignMaxPer> campaignMaxPerRepository,
            IRepository<Lookup, int> lookupRepository, IRedisIDMSConfigurationCache idmsConfigurationCache,
            IDatabaseRepository databaseRepository, IHostingEnvironment env, IRepository<AutoSuppress, Guid> autoSuppressRepository,
            IRepository<BuildTableLayout> buildTableLayoutRepository, IRepository<BuildTable, int> buildTableRepository,
            IRepository<Campaign, int> campaignRepository,
            IRepository<Build, int> buildRepository,
            IRepository<ExternalBuildTableDatabase> externalBuildTableDatabaseRepository,
            IRepository<CampaignExportLayout> campaignExportLayoutRepository,
            IRedisLookupCache lookupCache,
            ISegmentSelectionsAppService segmentSelectionsAppService
        )
        {
            _campaignRepository = campaignRepository;
            _segmentRepository = segmentRepository;
            _customSegmentRepository = customSegmentRepository;
            _campaignStatusRepository = campaignStatusRepository;
            _mySession = mySession;
            _orderStatusManager = orderStatusManager;
            _segmentBizness = segmentBizness;
            _mySession = mySession;
            _campaignMaxPerRepository = campaignMaxPerRepository;
            _lookupRepository = lookupRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
            _databaseRepository = databaseRepository;
            _appConfiguration = env.GetAppConfiguration();
            _webRootPath = env.WebRootPath;
            _autoSuppressRepository = autoSuppressRepository;
            _buildTableRepository = buildTableRepository;
            _buildRepository = buildRepository;
            _externalBuildTableDatabaseRepository = externalBuildTableDatabaseRepository;
            _buildTableLayoutRepository = buildTableLayoutRepository;
            _campaignExportLayoutRepository = campaignExportLayoutRepository;
            _lookupCache = lookupCache;
            _segmentSelectionsAppService = segmentSelectionsAppService;
        }

        public async Task<GetAllSegmentForCampaign> GetAllSegmentList(GetSegmentListInput input)
        {
            try
            {
                var queryForSegmentList = _segmentBizness.GetAllSegmentsList(input);
                var pagedSegments = await _customSegmentRepository.GetAllSegmentsList(input, queryForSegmentList.Item1, queryForSegmentList.Item2, queryForSegmentList.Item3);
                var currentStatus = _campaignStatusRepository.FirstOrDefault(o => o.OrderID == Convert.ToInt32(input.OrderId) && o.iIsCurrent)?.iStatus ?? 0;
                if (currentStatus == 0) throw new UserFriendlyException(L("campaignDeleted"));
                return new GetAllSegmentForCampaign
                {
                    CurrentStatus = currentStatus,
                    PagedSegments = pagedSegments
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public GetSegmentListForView GetSegmentsBasedOnIds(int segmentIds)
        {
            var segment = _customSegmentRepository.Get(segmentIds);
            var segmentItem = new GetSegmentListForView()
            {
                Id = segment.Id,
                OrderId = segment.OrderId,
                Description = segment.cDescription,
                cMaxPerGroup = segment.cMaxPerGroup,
                iProvidedQty = segment.iProvidedQty,
                iRequiredQty = segment.iRequiredQty,
                cKeyCode1 = segment.cKeyCode1,
                cKeyCode2 = segment.cKeyCode2,
                iDedupeOrderSpecified = segment.iDedupeOrderSpecified,
                iGroup = segment.iGroup
            };
            return segmentItem;

        }

        public GetAllSegmentsDropdownOutputDto GetAllSegmentForDropdown(int campaignId, int databaseId)
        {
            try
            {
                var currentStatus = _campaignStatusRepository.FirstOrDefault(o => o.OrderID == campaignId && o.iIsCurrent)?.iStatus ?? 0;
                if (currentStatus == 0) throw new UserFriendlyException(L("campaignDeleted"));
                var dropdownValues = _segmentRepository.GetAll()
                                        .Where(seg => Convert.ToInt32(seg.OrderId) == campaignId)
                                        .OrderBy(seg => seg.iDedupeOrderSpecified)
                                        .Select(seg => new DropdownOutputDto()
                                        {
                                            Value = seg.Id,
                                            Label = string.IsNullOrWhiteSpace(seg.cKeyCode1) ? $"{seg.iDedupeOrderSpecified} : {seg.cDescription}" : $"{seg.iDedupeOrderSpecified} : {seg.cDescription} - {seg.cKeyCode1}",
                                        }).ToList();

                var orderLevelSelection = dropdownValues.FirstOrDefault(seg => seg.Label.Contains("0 :"));
                var configQuickCount = _idmsConfigurationCache.GetConfigurationValue("QuickCountButton", databaseId).iValue == 1 ? true : false;
                var isSICScreenConfigured = _idmsConfigurationCache.GetConfigurationValue("SearchSICIndustry", databaseId).iValue == 1 ? true : false;
                var isCountyCityScreenConfigured = _idmsConfigurationCache.GetConfigurationValue("StateCitySelection", databaseId).iValue == 1 ? true : false;
                var isGeoSearchConfigured = _idmsConfigurationCache.GetConfigurationValue("SearchGeoRadius", databaseId).iValue == 1 ? true : false;
                var isOccupationScreenConfigured = _idmsConfigurationCache.GetConfigurationValue("OccupationSelection", databaseId).iValue == 1 ? true : false;


                if (orderLevelSelection != null) orderLevelSelection.Label = "0 : Common Rules";

                return new GetAllSegmentsDropdownOutputDto
                {
                    CurrentStatus = currentStatus,
                    SegmentDropDown = dropdownValues,
                    IsQuickCountButtonVisible = configQuickCount,
                    IsSICScreenConfigured = isSICScreenConfigured,
                    IsCountyCityScreenConfigured = isCountyCityScreenConfigured,
                    IsGeoSearchConfigured = isGeoSearchConfigured,
                    IsOccupationScreenConfigured = isOccupationScreenConfigured
                };
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Segments_Edit, AppPermissions.Pages_PlaceOrder)]
        public CreateOrEditSegmentDto GetSegmentForEdit(int segmentId)
        {
            var segment = new CreateOrEditSegmentDto();
            if (segmentId != 0)
            {
                var segmentForEdit = _segmentRepository.Get(segmentId);

                var segmentPriorityField = GetSelectionFieldForSegmentPriorityField(segmentForEdit);

                segment = new CreateOrEditSegmentDto()
                {
                    Id = segmentForEdit.Id,
                    OrderId = segmentForEdit.OrderId,
                    iDedupeOrderSpecified = segmentForEdit.iDedupeOrderSpecified,
                    cDescription = segmentForEdit.cDescription,
                    iRequiredQty = segmentForEdit.iRequiredQty,
                    iProvidedQty = segmentForEdit.iProvidedQty,
                    cKeyCode1 = segmentForEdit.cKeyCode1,
                    cKeyCode2 = segmentForEdit.cKeyCode2,
                    cMaxPerGroup = segmentForEdit.cMaxPerGroup,
                    iUseAutosuppress = segmentForEdit.iUseAutosuppress,
                    iGroup = segmentForEdit.iGroup,
                    ApplyDefaultRules = GetIsAutoSupress(segmentForEdit.OrderId),
                    iOutputQty = segmentForEdit.iOutputQty.Equals(-1) ? (segmentForEdit.iProvidedQty > 0 ? segmentForEdit.iProvidedQty : segmentForEdit.iOutputQty) : segmentForEdit.iOutputQty,
                    iIsCalculateDistance = FetchConfigValueForCalculateDistance(segmentForEdit.OrderId),
                    iIsRandomRadiusNth = segmentForEdit.iIsRandomRadiusNth,
                    cNthPriorityField = segmentForEdit.cNthPriorityField,
                    cNthPriorityFieldDescription = segmentPriorityField != null ? segmentPriorityField.Label : "",
                    cNthPriorityFieldOrder = segmentPriorityField != null ? segmentForEdit.cNthPriorityFieldOrder : ""
                };
            }
            return segment;
        }

        private ColumnDefinition GetSelectionFieldForSegmentPriorityField(Segment segmentForEdit)
        {
            if (!string.IsNullOrEmpty(segmentForEdit.cNthPriorityField))
            {
                var campaign = _campaignRepository.GetAll().Where(p => p.Id == segmentForEdit.OrderId).FirstOrDefault();
                var databaseId = _buildRepository.GetAll().Where(p => p.Id == campaign.BuildID).FirstOrDefault().DatabaseId;
                var selectionsFields = _segmentSelectionsAppService.GetSelectionFieldsNew(segmentForEdit.OrderId, "1", databaseId ?? 0, campaign.BuildID, campaign.MailerID);

                var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var segmentPriorityFields = JsonConvert.DeserializeObject<List<ColumnDefinition>>(selectionsFields.FilterDetails, jsonSerializerSettings);

                var nthPriorityFields = segmentForEdit.cNthPriorityField.Split('.');
                return segmentPriorityFields.FirstOrDefault(item => item.Field == nthPriorityFields[1] && item.Data.cTableName.Contains(nthPriorityFields[0]));
            }

            return null;
        }

        public bool GetIsAutoSupress(int campaignId)
        {
            try
            {
                var buildId = _campaignRepository.GetAll().Where(p => p.Id == campaignId).FirstOrDefault().BuildID;
                var databaseId = _buildRepository.GetAll().Where(p => p.Id == buildId).FirstOrDefault().DatabaseId;
                var isAutoSupress = from btl in _buildTableLayoutRepository.GetAll()
                                    join bt in _buildTableRepository.GetAll()
                                    on btl.BuildTableId equals bt.Id
                                    join b in _buildRepository.GetAll()
                                    on bt.BuildId equals b.Id
                                    join aus in _autoSuppressRepository.GetAll()
                                    on btl.cFieldName equals aus.cQuestionFieldName
                                    where b.Id == buildId && aus.iIsActive && aus.DatabaseID == databaseId
                                    select new
                                    {
                                        btl.cFieldDescription,
                                        aus
                                    };
                return isAutoSupress != null ? (isAutoSupress.Count() > 0 ? true : false) : false;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public GetAllSegmentsDropdownOutputDto GetMaxPerGroups(int campaignId, int segmentId)
        {
            try
            {
                var currentStatus = _campaignStatusRepository.FirstOrDefault(o => o.OrderID == campaignId && o.iIsCurrent)?.iStatus ?? 0;
                if (currentStatus == 0) throw new UserFriendlyException(L("campaignDeleted"));
                var lstLookup = _lookupCache.GetLookUpFields("MAXPER");
                var dropdownValues = lstLookup.Select(seg => new DropdownOutputDto()
                {
                    Value = seg.cCode,
                    Label = seg.cDescription
                }).ToList();
                return new GetAllSegmentsDropdownOutputDto
                {
                    CurrentStatus = currentStatus,
                    SegmentDropDown = dropdownValues,
                    DefaultMaxPerValue = "00"
                };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public int GetLatestIdedupeorderSpecified(int orderId)
        {
            var idedupe = 0;
            var record = _segmentRepository.GetAll().Where(x => !x.iIsOrderLevel && x.OrderId == orderId);
            if (record != null && record.Count() > 0)
            {
                idedupe = record.Max(x => x.iDedupeOrderSpecified);

            }
            return idedupe + 1;
        }

        public bool FetchConfigValueForCalculateDistance(int campaignId)
        {
            try
            {
                var databaseID = _databaseRepository.GetDataSetDatabaseByOrderID(campaignId).Id;
                return _idmsConfigurationCache.GetConfigurationValue("CalculateDistance", databaseID).cValue == "0" ? true : false;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message); ;
            }

        }



        public async Task<CreateOrEditSegmentOutputDto> CreateOrEdit(CreateOrEditSegmentDto segment, int orderStatus)
        {
            var result = new CreateOrEditSegmentOutputDto
            {
                Id = segment.Id == null ? 0 : segment.Id.GetValueOrDefault(),
                NewStatus = 0
            };
            if(segment.cNthPriorityFieldOrder  == null) {
                segment.cNthPriorityFieldOrder = "";
            }
            if(segment.cNthPriorityField == null) {
                segment.cNthPriorityField = "";
            }
            if (segment.Id == null)
            {
                result.Id = await CreateAsync(segment);
            }
            else
            {
                await UpdateAsync(segment);
            }
            if(orderStatus > 0)
            {
                var newStatus = orderStatus.Equals(Convert.ToInt32(CampaignStatus.OrderFailed)) ? CampaignStatus.OrderFailed : (orderStatus.Equals(Convert.ToInt32(CampaignStatus.OrderCompleted))) ? CampaignStatus.OrderCompleted : (orderStatus.Equals(Convert.ToInt32(CampaignStatus.OutputCompleted))) ? CampaignStatus.OrderCompleted : (orderStatus.Equals(Convert.ToInt32(CampaignStatus.OutputFailed))) ? CampaignStatus.OrderCompleted : CampaignStatus.OrderCreated;
                await _orderStatusManager.UpdateOrderStatus(segment.OrderId, newStatus, _mySession.IDMSUserName);
                result.NewStatus = (int)newStatus;
            }
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Segments_Create)]
        private async Task<int> CreateAsync(CreateOrEditSegmentDto input)
        {

            try
            {
                var idedupe = 0;
                var record = _segmentRepository.GetAll().Where(x => !x.iIsOrderLevel && x.OrderId == input.OrderId);
                if (record != null && record.Count() > 0)
                {
                    idedupe = record.Max(x => x.iDedupeOrderSpecified);
                }
                var newSegmentLevelSegment = new Segment()
                {
                    OrderId = input.OrderId,
                    cDescription = input.cDescription.Trim(),
                    iDedupeOrderSpecified = idedupe + 1,
                    cTitleSuppression = "B",
                    cCreatedBy = _mySession.IDMSUserName,
                    //iGroup = 1,
                    iIsRandomRadiusNth = input.iIsRandomRadiusNth,
                    iIsOrderLevel = false,
                    iUseAutosuppress = input.ApplyDefaultRules ? input.iUseAutosuppress : false,
                    cKeyCode1 = string.IsNullOrEmpty(input.cKeyCode1) ? string.Empty : input.cKeyCode1,
                    cKeyCode2 = string.IsNullOrEmpty(input.cKeyCode2) ? string.Empty : input.cKeyCode2,
                    iRequiredQty = input.iRequiredQty,
                    iProvidedQty = 0,
                    iDedupeOrderUsed = 0,
                    cMaxPerGroup = input.cMaxPerGroup != null ? input.cMaxPerGroup : "00",
                    cFixedTitle = string.Empty,
                    iDoubleMultiBuyerCount = 0,
                    dCreatedDate = DateTime.Now,
                    iOutputQty = -1,
                    iAvailableQty = 0,
                    iGroup = input.iGroup,
                    cNthPriorityField = input.cNthPriorityField,
                    cNthPriorityFieldOrder = input.cNthPriorityFieldOrder
                };

                var segmentID = await _segmentRepository.InsertAndGetIdAsync(newSegmentLevelSegment);
                CurrentUnitOfWork.SaveChanges();
                return segmentID;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Segments_Edit)]
        private async Task UpdateAsync(CreateOrEditSegmentDto input)
        {
            try
            {
                var segment = _segmentRepository.Get(input.Id.Value);
                segment.cDescription = input.cDescription;
                segment.iRequiredQty = input.iRequiredQty;
                segment.dModifiedDate = DateTime.Now;
                segment.cModifiedBy = _mySession.IDMSUserName;
                segment.cKeyCode1 = string.IsNullOrEmpty(input.cKeyCode1) ? string.Empty : input.cKeyCode1;
                segment.cKeyCode2 = string.IsNullOrEmpty(input.cKeyCode2) ? string.Empty : input.cKeyCode2;
                segment.cMaxPerGroup = input.cMaxPerGroup != null ? input.cMaxPerGroup : "00";
                segment.iUseAutosuppress = input.ApplyDefaultRules ? input.iUseAutosuppress : false;
                segment.iGroup = input.iGroup;
                segment.iOutputQty = (input.iOutputQty.Equals(segment.iProvidedQty) || input.iOutputQty.Equals(null)) ? -1 : (int)input.iOutputQty;
                segment.iIsRandomRadiusNth = input.iIsRandomRadiusNth;
                segment.cNthPriorityField = input.cNthPriorityField;
                segment.cNthPriorityFieldOrder = input.cNthPriorityFieldOrder;
                await _segmentRepository.UpdateAsync(segment);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #region Quick Count
        public async Task<string> GetQuickCountAsync(int segmentID)
        {
            try
            {
                var iCount = 0;
                var quickCountResult = string.Empty;
                var endpointaddress = _appConfiguration["Services:Uri"];
                var service = new IDMSCommonService.IDMSIQServiceClient(endpointaddress);
                var response = await service.GetQuickCountAsync(segmentID);
                if (response.GetQuickCountResult.IsSuccess)
                {
                    iCount = response.GetQuickCountResult.Count;
                    quickCountResult = String.Format("{0:N0}", iCount);
                }
                else
                {
                    throw new UserFriendlyException(response.GetQuickCountResult.Error.Message);
                }

                var isOrderInStatusToUpdate = _customSegmentRepository.IsOrderInStatusToUpdate(segmentID);
                if (isOrderInStatusToUpdate)
                    await UpdateQuickCountAsync(iCount, segmentID);
                return quickCountResult;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        private async Task UpdateQuickCountAsync(int quickCount, int segmentID)
        {
            try
            {
                var segment = _segmentRepository.Get(segmentID);
                segment.iAvailableQty = quickCount;
                await _segmentRepository.UpdateAsync(segment);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Segments_Delete)]
        public async Task Delete(EntityDto input)
        {
            try
            {
                var segment = await _segmentRepository.FirstOrDefaultAsync(input.Id);
                var campaignID = segment != null ? segment.OrderId : 0;
                await _customSegmentRepository.DeleteSegmentAsync(input.Id);
                await _orderStatusManager.UpdateOrderStatus(campaignID, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        #region Copy Segment
        [AbpAuthorize(AppPermissions.Pages_Segments_Copy)]
        public async Task<int> Copy(CopySegmentDto input)
        {
            try
            {
                if (input.iSegmentId == 0) throw new UserFriendlyException(L("segmentDeleted"));
                input.sInitiatedBy = _mySession.IDMSUserName;
                input.sCommaSeparatedSegments = SplitCommaSeparatedString(input.iNumberOfCopies, input.cCopyMode, input.iMaxDedupeId, input.iSegmentFrom, input.iSegmentTo);
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                var segmentID = await _customSegmentRepository.CopySegmentAsync(input);
                await _orderStatusManager.UpdateOrderStatus(input.iCampaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                return segmentID;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        #endregion

        #region Import Segment
        [AbpAuthorize(AppPermissions.Pages_Segments_ImportSegments)]
        public async Task<int> ImportSegment(ImportSegmentDTO input)
        {
            try
            {
                if (input.NumberOfSegments.Equals(0) || input.cSegmentDescription.Equals("NOT FOUND"))
                    throw new UserFriendlyException(L("CampaignNotFoundValidation"));

                input.sCommaSeparatedSegments = CommonHelpers.GetSplitCommaSeparatedString(input.sCommaSeparatedSegments, input.NumberOfSegments);

                if (string.IsNullOrWhiteSpace(input.sCommaSeparatedSegments))
                    throw new UserFriendlyException(L("RangeValidation"));

                var numberofSegment = await _customSegmentRepository.CopySegmentFromCampaign(input.iCopyToCampaignID, input.iCopyFromCampaignID, input.sCommaSeparatedSegments, _mySession.IDMSUserName);
                if (numberofSegment > 0)
                    await _orderStatusManager.UpdateOrderStatus(input.iCopyToCampaignID, CampaignStatus.OrderCreated, _mySession.IDMSUserName);

                return numberofSegment;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public async Task<ImportSegmentDTO> ValidateCampaignIDForImportSegment(int iCopyToOrderID, int iCopyFromOrderID)
        {
            try
            {
                return await _customSegmentRepository.ImportSegmentValidationAsync(iCopyToOrderID, iCopyFromOrderID, _mySession.IDMSUserId);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }


        #endregion

        private string SplitCommaSeparatedString(int numberOfCopies, string copyMode, int maxDedupe, int fromSegment, int toSegment)
        {
            var sValues = new StringBuilder();
            var formatList = "{0},";

            for (int j = 1; j <= numberOfCopies; j++)
            {
                if (copyMode == "all")
                {
                    for (int i = 1; i <= maxDedupe; i++)
                    {
                        sValues.Append(string.Format(formatList, i.ToString()));
                    }
                }
                else if (copyMode == "from")
                {
                    if (fromSegment > toSegment)
                    {
                        throw new UserFriendlyException(L("FromToValidation"));
                    }
                    for (int i = fromSegment; i <= toSegment; i++)
                    {
                        sValues.Append(string.Format(formatList, i.ToString()));
                    }
                }
            }
            return sValues.ToString();
        }

        public int GetMaximumIDedupeNumber(int orderId)
        {
            try
            {
                return _customSegmentRepository.GetAll().Where(x => x.OrderId == orderId).Max(x => x.iDedupeOrderSpecified);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void MoveSegment(int segmentid, int fromSegment, int toSegment, int toLocation, int campaignId)
        {
            try
            {
                var initiatedBy = _mySession.IDMSUserName;
                if (fromSegment > toSegment)
                {
                    throw new UserFriendlyException(L("FromToValueException"));
                }

                _customSegmentRepository.MoveSegment(segmentid, fromSegment, toSegment, toLocation, initiatedBy);
                _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<GetSegmentListForView> GetSegmentForView(int Id)
        {
            try
            {
                return await _customSegmentRepository.GetSegmentsForViewById(_segmentBizness.GetSegmentForViewByIdQuery(Id));
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

    }
}