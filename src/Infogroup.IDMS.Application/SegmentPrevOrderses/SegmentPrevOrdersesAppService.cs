using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SegmentPrevOrderses.Dtos;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Sessions;
using Abp.UI;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.IDMSConfigurations;

namespace Infogroup.IDMS.SegmentPrevOrderses
{
    [AbpAuthorize(AppPermissions.Pages_SegmentPrevOrderses)]
    public class SegmentPrevOrdersesAppService : IDMSAppServiceBase, ISegmentPrevOrdersesAppService
    {
        private readonly IRepository<SegmentPrevOrders> _segmentPrevOrdersRepository;
        private readonly IRepository<OrderStatus, int> _campaignStatusRepository;
        private readonly AppSession _mySession;
        private readonly IDatabaseRepository _databaseRpository;
        private readonly ISegmentPreviousOrderRepository _customSegmentPrevOrdersRepository;
        private readonly IOrderStatusManager _orderStatusManager;
        private readonly IRepository<Campaign, int> _campaignRepository;
        private readonly IShortSearch _shortSearch;
        private readonly IBuildTableLayoutRepository _customBuildTableLayoutRepository;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;

        private readonly string IsDivisionalDBConfig = "UseDivisionalCustomer";

        public SegmentPrevOrdersesAppService(
              IRepository<SegmentPrevOrders> segmentPrevOrdersRepository,
              ISegmentPreviousOrderRepository customSegmentPrevOrdersRepository,
              IRepository<Campaign, int> campaignRepository,
              AppSession mySession,
              IDatabaseRepository databaseRepository,
              IRepository<OrderStatus, int> campaignStatusRepository,
              IOrderStatusManager orderStatusManager,
              IShortSearch shortSearch,
              IBuildTableLayoutRepository customBuildTableLayoutRepository,
              IRedisIDMSConfigurationCache idmsConfigurationCache
               )
        {
            _segmentPrevOrdersRepository = segmentPrevOrdersRepository;
            _customSegmentPrevOrdersRepository = customSegmentPrevOrdersRepository;
            _campaignRepository = campaignRepository;
            _databaseRpository = databaseRepository;
            _mySession = mySession;
            _orderStatusManager = orderStatusManager;
            _campaignStatusRepository = campaignStatusRepository;
            _shortSearch = shortSearch;
            _customBuildTableLayoutRepository = customBuildTableLayoutRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
        }

        #region Get All PrevOrders        
        public async Task<List<GetSegmentPrevOrdersForViewDto>> GetAllPrevOrdersList(GetPreviousOrdersFilters filters)
        {
            try
            {
                var isDivisional = false;
                int divisionId;
                var Id = 0;
                var shortWhere = _shortSearch.GetWhere(PageID.CampaignHistory, filters.filter);
                var result = _databaseRpository.GetDataSetDatabaseByOrderID(filters.CampaignId);
                var configCValue = _idmsConfigurationCache.GetConfigurationValue(IsDivisionalDBConfig).cValue;
                var databaseID = _databaseRpository.GetDatabaseIDByOrderID(filters.CampaignId).Id;
                var defaultMatchLevel = _idmsConfigurationCache.GetConfigurationValue("MatchLevel", databaseID).cValue;
                if (string.IsNullOrEmpty(configCValue))
                {
                    isDivisional = false;
                    Id = result.Id;
                }
                else
                {
                    isDivisional = configCValue.Split(',')
                       .Select(id => { int.TryParse(id, out divisionId); return divisionId; })
                       .Any(id => id == result.DivisionId);

                    if (isDivisional)
                        Id = (int)result.DivisionId;
                    else
                        Id = result.Id;
                }
                return await _customSegmentPrevOrdersRepository.GetAllPreviousOrders(Id, filters, _mySession.IDMSUserId, shortWhere, isDivisional, defaultMatchLevel);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public SegmentPrevOrdersView GetExistingPreviousOrders(int campaignID, int segmentID)
        {
            var segmentPrevOrder = new SegmentPrevOrdersView();
            try
            {
                segmentPrevOrder.CurrentStatus = _campaignStatusRepository.FirstOrDefault(o => o.OrderID == campaignID && o.iIsCurrent)?.iStatus ?? 0;
                if (segmentPrevOrder.CurrentStatus == 0) throw new UserFriendlyException(L("campaignDeleted"));
                var query = (from prevOrder in _segmentPrevOrdersRepository.GetAll()
                             join campaign in _campaignRepository.GetAll()
                             on prevOrder.PrevOrderID equals campaign.Id
                             where prevOrder.SegmentId.Equals(segmentID)

                             select new GetSegmentPrevOrdersForViewDto
                             {
                                 PreviousOrderID = prevOrder.Id,
                                 PrevSegmentIDs = prevOrder.cPrevSegmentID,
                                 OrderID = campaign.Id,
                                 Description = campaign.cDescription,
                                 cIncludeOrExclude = prevOrder.cIncludeExclude.Equals("E") ? "Exclude" : "Include",
                                 cIndividualOrCompany = prevOrder.cCompareFieldName.Equals("I") ? "Individual" : "Company",
                                 action = ActionType.None,
                                 cLVAOrderNo = campaign.cLVAOrderNo,
                                 cMatchFieldName = prevOrder.cMatchFieldName,
                                 BuildId=campaign.BuildID
                             });

                segmentPrevOrder.listOfSegmentPrevOrders = query.OrderBy(o => o.OrderID).ToList();

                foreach (var item in segmentPrevOrder.listOfSegmentPrevOrders)
                {
                    var refineditem = CommonHelpers.ConvertNullStringToEmptyAndTrim(item);
                    if (string.IsNullOrEmpty(refineditem.cMatchFieldName))
                    {
                        item.cMatchFieldDescription = string.Empty;
                    }
                    else
                    {
                        item.cMatchFieldDescription = GetFieldNamesBasedOnBuildId("", refineditem.BuildId).FirstOrDefault(x => x.Value.ToString() == refineditem.cMatchFieldName).Label;
                    }
                }

                return segmentPrevOrder;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public List<string> GetListOfSegmentIDs(int previousOrderID)
        {
            try
            {
                var segments = _segmentPrevOrdersRepository.Query(p => p.Where(q => q.Id.Equals(previousOrderID))).Select(a => a.cPrevSegmentID).ToList().FirstOrDefault();
                var listOfPreviousSegmentIds = new List<string>();
                if (!string.IsNullOrEmpty(segments))
                {
                    listOfPreviousSegmentIds = segments.Split(",").ToList();
                }
                return listOfPreviousSegmentIds;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
        #region CreateEditPreviousOrders
        [AbpAuthorize(AppPermissions.Pages_SegmentPrevOrderses_Edit)]
        public async Task SavePreviousOrders(CreateOrEditSegmentPrevOrdersDto input)
        {
            try
            {
                if (input.listOfSegmentOrders != null && input.listOfSegmentOrders.Count > 0)
                {
                    var listOfSegmentOrders = input.listOfSegmentOrders.Where(xta => !xta.action.Equals(ActionType.None)).ToList();
                    CheckPreviousOrdersValidations(input.CampaignID, listOfSegmentOrders);
                    foreach (var previousSegmentOrder in listOfSegmentOrders)
                    {
                        switch (previousSegmentOrder.action)
                        {
                            case ActionType.Add:
                                var savePreviousOrderData = new SegmentPrevOrders();
                                savePreviousOrderData = new SegmentPrevOrders
                                {

                                    PrevOrderID = previousSegmentOrder.PrevOrderID,
                                    SegmentId = previousSegmentOrder.SegmentId,
                                    cPrevSegmentID = previousSegmentOrder.cPrevSegmentID,
                                    cIncludeExclude = previousSegmentOrder.cIncludeExclude.Equals("Exclude") ? "E" : "I",
                                    cCompareFieldName = previousSegmentOrder.cCompanyFieldName.Equals("Individual") ? "I" : "C",
                                    cPrevSegmentNumber = previousSegmentOrder.cPrevSegmentNumber,
                                    cMatchFieldName = previousSegmentOrder.cMatchFieldName,
                                    cCreatedBy = _mySession.IDMSUserName,
                                    dCreatedDate = DateTime.Now
                                };
                                await _segmentPrevOrdersRepository.InsertAsync(savePreviousOrderData);
                                break;
                            case ActionType.Edit:
                                var updatePrevOrder = _segmentPrevOrdersRepository.Get(previousSegmentOrder.Id);
                                updatePrevOrder.cIncludeExclude = previousSegmentOrder.cIncludeExclude.Equals("Exclude") ? "E" : "I";
                                updatePrevOrder.cCompareFieldName = previousSegmentOrder.cCompanyFieldName.Equals("Individual") ? "I" : "C";
                                updatePrevOrder.cPrevSegmentID = previousSegmentOrder.cPrevSegmentID;
                                updatePrevOrder.cPrevSegmentNumber = previousSegmentOrder.cPrevSegmentNumber;
                                updatePrevOrder.cMatchFieldName = previousSegmentOrder.cMatchFieldName;
                                updatePrevOrder.dModifiedDate = DateTime.Now;
                                updatePrevOrder.cModifiedBy = _mySession.IDMSUserName;
                                await _segmentPrevOrdersRepository.UpdateAsync(updatePrevOrder);
                                break;
                            case ActionType.Delete:
                                if (!previousSegmentOrder.Id.Equals(0))
                                    await _segmentPrevOrdersRepository.DeleteAsync(previousSegmentOrder.Id);
                                break;
                            default:
                                throw new Exception("Unexpected Case");
                        }
                    }
                    await _orderStatusManager.UpdateOrderStatus(input.CampaignID, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                }
            }
            catch (Exception ex)

            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        private void CheckPreviousOrdersValidations(int campaignID, List<SegmentPrevOrdersDto> listOfSegmentOrders)
        {
            var lstKeyMisMatchOrderIds = new List<string>();
            var strField = string.Empty;
            var strFieldDesc = string.Empty;
            foreach (var segmentPrevOrder in listOfSegmentOrders)
            {
                strField = _customSegmentPrevOrdersRepository.ValidateKeyColumnWithPrevOrders(campaignID.ToString(), segmentPrevOrder.PrevOrderID.ToString());
                if (!string.IsNullOrEmpty(strField))
                {
                    strFieldDesc = strField;
                    lstKeyMisMatchOrderIds.Add(segmentPrevOrder.PrevOrderID.ToString());
                }
            }
            if (lstKeyMisMatchOrderIds != null && lstKeyMisMatchOrderIds.Count > 0)
            {
                var strOrderIds = string.Join(",", lstKeyMisMatchOrderIds.ToArray());
                throw new UserFriendlyException(L("PrevValidationMessage", strOrderIds, strFieldDesc.ToString()));
            }
        }
        #endregion

        public List<DropdownOutputDto> GetFieldNamesBasedOnBuildId(string filter, int buildId)
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                {
                    return _customBuildTableLayoutRepository.GetBuildTableLayoutFieldByBuildID(buildId.ToString());
                }
                else
                    return _customBuildTableLayoutRepository.GetBuildTableLayoutFieldByBuildID(buildId.ToString()).Where(x => x.Label.ToLower().Contains(filter.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}