using Infogroup.IDMS.Databases;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SavedSelections.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Abp.UI;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.Segments;
using Infogroup.IDMS.SegmentSelections;
using Infogroup.IDMS.UserSavedSelectionDetails;
using Infogroup.IDMS.SavedSelectionDetails;
using Infogroup.IDMS.UserSavedSelections;
using Infogroup.IDMS.OrderStatuss;

namespace Infogroup.IDMS.SavedSelections
{
    [AbpAuthorize(AppPermissions.Pages_SavedSelections)]
    public class SavedSelectionsAppService : IDMSAppServiceBase, ISavedSelectionsAppService
    {
        private readonly ISavedSelectionRepository _customSavedSelectionRepository;
        private readonly IDatabaseRepository _customDatabaseRepository;
        private readonly IRepository<Campaign, int> _campaignRepository;
        private readonly IRepository<Build, int> _buildRepository;
        private readonly IRepository<BuildTable, int> _buildTableRepository;
        private readonly IRepository<Segment, int> _segmentRepository;
        private readonly IRepository<SegmentSelection, int> _segmentSelectionRepository;
        private readonly IRepository<UserSavedSelectionDetail, int> _userSavedSelectionDetailRepository;
        private readonly IRepository<SavedSelectionDetail, int> _savedSelectionDetailRepository;
        private readonly IRepository<UserSavedSelection, int> _userSavedSelectionRepository;
        private readonly AppSession _mySession;
        private readonly IOrderStatusManager _orderStatusManager;

        public SavedSelectionsAppService(
            ISavedSelectionRepository customSavedSelectionRepository,
            AppSession mySession,
            IDatabaseRepository customDatabaseRepository,
            IRepository<Campaign, int> campaignRepository,
            IRepository<Build, int> buildRepository,
            IRepository<BuildTable, int> buildTableRepository,
            IRepository<Segment, int> segmentRepository,
            IRepository<SegmentSelection, int> segmentSelectionRepository,
            IRepository<UserSavedSelectionDetail, int> userSavedSelectionDetailRepository,
            IRepository<SavedSelectionDetail, int> savedSelectionDetailRepository,
            IRepository<UserSavedSelection, int> userSavedSelectionRepository,
            IOrderStatusManager orderStatusManager)
        {
            _customSavedSelectionRepository = customSavedSelectionRepository;
            _customDatabaseRepository = customDatabaseRepository;
            _campaignRepository = campaignRepository;
            _buildRepository = buildRepository;
            _buildTableRepository = buildTableRepository;
            _segmentRepository = segmentRepository;
            _segmentSelectionRepository = segmentSelectionRepository;
            _userSavedSelectionDetailRepository = userSavedSelectionDetailRepository;
            _savedSelectionDetailRepository = savedSelectionDetailRepository;
            _userSavedSelectionRepository = userSavedSelectionRepository;
            _mySession = mySession;
            _orderStatusManager = orderStatusManager;
        }

        [AbpAuthorize(AppPermissions.Pages_SavedSelections)]
        public async Task<PagedResultDto<GetSavedSelectionForViewDto>> GetAllSavedSelections(GetAllSavedSelectionsInput input)
        {
            try
            {
                return await _customSavedSelectionRepository.GetAllSavedSelection(_customDatabaseRepository.GetDataSetDatabaseByOrderID(input.CampaignID).Id, _mySession.IDMSUserId, input);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public void AddSavedSelectionsAsync(AddSavedSelection input)
        {
            var buildTable = BuildTablebySegmentID(input.SegmentID);
            var maxGrouping = _segmentSelectionRepository.GetAll().Where(a => a != null && a.SegmentId == input.SegmentID && a.iGroupNumber != 999).DefaultIfEmpty().Max(a => a.iGroupNumber);
            
            foreach (var item in input.SavedSelectionList)
            {
                var segmentSelectionList = new List<SegmentSelection>();

                if (item.UserDefault)
                {
                    segmentSelectionList = _userSavedSelectionDetailRepository.GetAll().Where(x => x.iIsActive && x.UserSavedSelectionId == item.ID).OrderBy(y => y.iGroupNumber).ThenBy(z => z.Id).ToList().Select(selection =>
                    {
                        return ObjectMapper.Map<SegmentSelection>(selection);                         
                    }).ToList(); 
                }
                else
                {
                    segmentSelectionList = _savedSelectionDetailRepository.GetAll().Where(x => x.iIsActive && x.SavedSelectionId == item.ID).OrderBy(y => y.iGroupNumber).ThenBy(z => z.Id).ToList().Select(selection =>
                    {
                        return ObjectMapper.Map<SegmentSelection>(selection);                        
                    }).ToList();                    
                }                

                var iGroupNo = 0;
                foreach (var row in segmentSelectionList)
                {
                    if (iGroupNo != row.iGroupNumber)
                    {
                        maxGrouping++;
                        iGroupNo = row.iGroupNumber;
                    }

                    if (row.cTableName == null || row.cTableName.Contains("Main"))
                        row.cTableName = buildTable;
                    else if (row.cTableName.Contains("Child"))
                    {
                        var sChildTable = row.cTableName.Split('_');
                        var mainTable = buildTable.Split('_');
                        row.cTableName = $"{sChildTable[0]}_{mainTable[1]}_{mainTable[2]}";
                    }
                    row.SegmentId = input.SegmentID;
                    row.iGroupNumber = maxGrouping;
                    row.cCreatedBy = _mySession.IDMSUserName;
                    row.dCreatedDate = DateTime.Now;
                    row.Id = 0;

                    _segmentSelectionRepository.Insert(row);
                    CurrentUnitOfWork.SaveChanges();
                    _orderStatusManager.UpdateOrderStatus(input.CampaignID, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                }
            }
        }
        
        private string BuildTablebySegmentID(int segmentID)
        {
            return (from campaign in _campaignRepository.GetAll()
                              join build in _buildRepository.GetAll() on campaign.BuildID equals build.Id
                              join buildTable in _buildTableRepository.GetAll() on new { a = build.Id, b = "M" } equals new { a = buildTable.BuildId, b = buildTable.LK_TableType }
                              join segment in _segmentRepository.GetAll() on campaign.Id equals segment.OrderId
                              where segment.Id == segmentID
                              select buildTable.cTableName).FirstOrDefault().ToString();
        }

        [AbpAuthorize(AppPermissions.Pages_SavedSelections_Delete)]
        public async void DeleteSavedSelectionAsync(int id)
        {
            try
            {
                await _customSavedSelectionRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public async void DeleteUserSavedSelectionAsync(int id)
        {
            try
            {
                await _userSavedSelectionRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void UpdateIsDefaultRule(int ruleId, bool isDefault)
        {
            try
            {
                var ruleForDefault = _userSavedSelectionRepository.GetAll().FirstOrDefault(x => x.Id == ruleId);
                ruleForDefault.iIsDefault = isDefault;
                ruleForDefault.cModifiedBy = _mySession.IDMSUserName;
                ruleForDefault.dModifiedDate = DateTime.Now;
                _userSavedSelectionRepository.Update(ruleForDefault);

                if(isDefault)
                {
                    var ruleForNotDefault = _userSavedSelectionRepository.GetAllList(x => x.DatabaseId == ruleForDefault.DatabaseId && x.cChannelType == ruleForDefault.cChannelType && x.UserID == _mySession.IDMSUserId && x.iIsActive && x.iIsDefault);
                    foreach (var item in ruleForNotDefault)
                    {
                        item.iIsDefault = false;
                        item.cModifiedBy = _mySession.IDMSUserName;
                        item.dModifiedDate = DateTime.Now;
                        _userSavedSelectionRepository.Update(item);
                    }
                }
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}