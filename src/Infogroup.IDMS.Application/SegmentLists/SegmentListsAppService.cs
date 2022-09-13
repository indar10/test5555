using Infogroup.IDMS.Segments;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SegmentLists.Dtos;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Infogroup.IDMS.MasterLoLs;
using Infogroup.IDMS.Shared.Dtos;
using Abp.UI;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.OrderStatuss;

namespace Infogroup.IDMS.SegmentLists
{
    [AbpAuthorize(AppPermissions.Pages_SegmentLists)]
    public class SegmentListsAppService : IDMSAppServiceBase, ISegmentListsAppService
    {
        private readonly IRepository<SegmentList> _segmentListRepository;
        private readonly IRepository<MasterLoL, int> _masterLoLRepository;
        private readonly IRepository<Segment, int> _segmentRepository;
        private readonly IRepository<Campaign, int> _campaignRepository;
        private readonly IRepository<OrderStatus, int> _campaignStatusRepository;
        private readonly ISegmentListRepository _customSegmentListRepository;
        private readonly AppSession _mySession;
        private readonly IOrderStatusManager _orderStatusManager;
        private readonly SegmentListBizness _segmentListBizness;


        public SegmentListsAppService(IRepository<SegmentList> segmentListRepository,
              IRepository<Campaign, int> campaignRepository,
              IRepository<OrderStatus, int> campaignStatusRepository,
              IRepository<Segment, int> segmentRepository,
              IRepository<MasterLoL, int> masterLoLRepository,
              ISegmentListRepository customSegmentListRepository,
              AppSession mySession,
              IOrderStatusManager orderStatusManager,
              SegmentListBizness segmentListBizness
            )
        {
            _segmentListRepository = segmentListRepository;
            _segmentRepository = segmentRepository;
            _campaignRepository = campaignRepository;
            _campaignStatusRepository = campaignStatusRepository;
            _masterLoLRepository = masterLoLRepository;
            _customSegmentListRepository = customSegmentListRepository;
            _mySession = mySession;
            _orderStatusManager = orderStatusManager;
            _segmentListBizness = segmentListBizness;
        }

        public async Task<List<SourceDto>> FetchApprovedSources(GetAllApprovedSourcesInput input)
        {
            try
            {
                var query = _segmentListBizness.GetApprovedSources(input);
                return await _customSegmentListRepository.GetApprovedSourcesAsync(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public GetExistingSourceDataForView GetExistingSourceData(int iSegmentID)
        {
            try
            {
                var result = new GetExistingSourceDataForView();
                var segment = _segmentRepository.Get(iSegmentID);
                if (segment == null) throw new UserFriendlyException(L("segmentDeleted"));
                result.CurrentStatus = _campaignStatusRepository.FirstOrDefault(o => o.OrderID == Convert.ToInt32(segment.OrderId) && o.iIsCurrent)?.iStatus ?? 0;
                if (result.CurrentStatus == 0) throw new UserFriendlyException(L("campaignDeleted"));
                result.CampaignLevel = segment.iIsOrderLevel;
                result.iDedupeOrderSpecified = segment.iDedupeOrderSpecified;
                result.SelectedSources = (from list in _masterLoLRepository.GetAll()
                                          join selectedList in _segmentListRepository.GetAll()
                                          on list.Id equals selectedList.MasterLOLID
                                          where selectedList.SegmentId.Equals(iSegmentID)
                                          orderby "ID ASC"
                                          select new SourceDto
                                          {
                                              ListID = list.Id,
                                              Id = selectedList.Id,
                                              ListName = list.cListName,
                                              Action = ActionType.None
                                          }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_SegmentLists_Edit)]
        public async Task SaveSources(SaveSourcesInputDto input)
        {
            try
            {
                var additionIDs = input.AddedSources.Select(segList => segList.ListID).ToList();
                if (additionIDs.Count > 0)
                    await _customSegmentListRepository.AddSourcesAsync(input.SegmentID, string.Join(',', additionIDs), _mySession.IDMSUserName);

                var deletionIDs = input.DeletedSources.Select(segList => segList.ListID).ToList();
                if (deletionIDs.Count > 0)
                    await _customSegmentListRepository.DeleteAsync(input.SegmentID, string.Join(',', deletionIDs));

                // Unlocking Campaign...
                var segment = _segmentRepository.Get(input.SegmentID);
                if (segment == null) throw new UserFriendlyException(L("segmentDeleted"));
                await _orderStatusManager.UpdateOrderStatus(segment.OrderId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}