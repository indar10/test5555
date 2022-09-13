using Infogroup.IDMS.SubSelects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SubSelectSelections.Dtos;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Infogroup.IDMS.Segments;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.SegmentSelections;
using Infogroup.IDMS.Sessions;
using Abp.UI;
using Infogroup.IDMS.OrderStatuss;

namespace Infogroup.IDMS.SubSelectSelections
{
    [AbpAuthorize(AppPermissions.Pages_SubSelects)]
    public class SubSelectSelectionsAppService : IDMSAppServiceBase, ISubSelectSelectionsAppService
    {
        private readonly ISubSelectSelectionsRepository _customSubSelectSelectionRepository;
        private readonly ISegmentRepository _customSegmentRepository;
        private readonly IBuildRepository _customBuildRepository;
        private readonly ISegmentSelectionRepository _customSegmentSelectionRepository;
        private readonly IOrderStatusManager _orderStatusManager;
        private readonly AppSession _mySession;

        public SubSelectSelectionsAppService(
              ISubSelectSelectionsRepository customSubSelectSelectionRepository,
              ISegmentRepository customSegmentRepository,
              IBuildRepository customBuildRepository,
              ISegmentSelectionRepository customSegmentSelectionRepository,
              IOrderStatusManager orderStatusManager,
              AppSession mySession)
        {
            _customSubSelectSelectionRepository = customSubSelectSelectionRepository;
            _customSegmentRepository = customSegmentRepository;
            _customBuildRepository = customBuildRepository;
            _orderStatusManager = orderStatusManager;
            _customSegmentSelectionRepository = customSegmentSelectionRepository;
            _mySession = mySession;
        }

        public SubSelectSelectionsDetailsDto GetAllSubSelectSelections(GetAllSubSelectSelectionsInput input)
        {
            try
            {
                var iBuildLoLID = 0;
                if (input.iSubSelectID > 0 && _customSegmentSelectionRepository.GetSubSelListCount(input.iSubSelectID) == 1)
                    iBuildLoLID = _customBuildRepository.GetSubSelBuildLolID(input.iSubSelectID, input.BuildId);

                var fields= _customSubSelectSelectionRepository.GetAllSubSelectSelections(input.iSubSelectID, iBuildLoLID);
                return new SubSelectSelectionsDetailsDto
                {
                    BuildLolId = iBuildLoLID,
                    Fields = fields
                };
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public async Task CreateOrEditSubSelectSelection(SubSelectSelectionsDTO input)
        {
            try
            {
                if (input.addedFilterId > 0)
                    _customSubSelectSelectionRepository.UpdateSubSelectSelection(input.addedFilterId, "Y");

                input.cCreatedBy = _mySession.IDMSUserName;
                input.dCreatedDate = DateTime.Now;
                input.iGroupOrder = 1;
                input.iGroupNumber = 1;
                var selection = ObjectMapper.Map<SubSelectSelection>(input);
                await _customSubSelectSelectionRepository.InsertAsync(selection);
                await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public async Task DeleteSubSelectSelection(int subSelectSelectionId, int addedFilterId, int campaignId)
        {
            try
            {
                if (addedFilterId > 0)
                    _customSubSelectSelectionRepository.UpdateSubSelectSelection(addedFilterId, "N");

                await _customSubSelectSelectionRepository.DeleteAsync(subSelectSelectionId);
                await _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);

            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
    }
}