using Abp.UI;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.SegmentSelections.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Infogroup.IDMS.Constants;

namespace Infogroup.IDMS.SegmentSelections
{
    [AbpAuthorize]
    public partial class SegmentSelectionsAppService : IDMSAppServiceBase, ISegmentSelectionsAppService
    {                           
        #region SAVE Selections from SIC Codes and County/City     
        public async Task SaveAdvanceSelection(int campaignId, AdvanceSelectionsInputDto input)
        {
            SegmentSelection selection;
            try
            {
                var NewGroupID = GetNewMaxGroupId(input.SegmentID);
                var cGrouping = input.SICFields.Count > 1 ? "Y" : "N";
                foreach (SegmentSelectionDto field in input.SICFields)
                {
                    selection = ObjectMapper.Map<SegmentSelection>(field);
                    selection.iGroupNumber = NewGroupID;
                    selection.cGrouping = cGrouping;
                    selection.cCreatedBy = _mySession.IDMSUserName;
                    selection.dCreatedDate = DateTime.Now;
                    await _segmentSelectionRepository.InsertAndGetIdAsync(selection);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                if (input.PrimarySICField != null)
                {
                    selection = ObjectMapper.Map<SegmentSelection>(input.PrimarySICField);
                    selection.iGroupNumber = NewGroupID + 1;
                    selection.cCreatedBy = _mySession.IDMSUserName;
                    selection.dCreatedDate = DateTime.Now;
                    await _segmentSelectionRepository.InsertAndGetIdAsync(selection);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                await _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.OrderCreated,_mySession.IDMSUserName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
        
        #region Geo Radius Screen   
        public async Task<List<AddressDetailDto>> GetAddressDetails(AddressInputDto input)
        {
            try
            {
                var result = new List<AddressDetailDto>();
                var endpointAddress = _appConfiguration["Services:Uri"];
                var service = new IDMSCommonService.IDMSIQServiceClient(endpointAddress);
                var response = await service.VerifyGeoRadiusAsync(input.AddressFilter, input.DatabaseId, input.MainTableName);
                var errorInfo = response.VerifyGeoRadiusResult.FirstOrDefault().Split(":");
                if (errorInfo[0] == "0")
                    throw new UserFriendlyException(errorInfo[1]);
                var addressess = response.VerifyGeoRadiusResult
                                    .Select(address =>
                                    {
                                        var addressDetails = address.Split(":");
                                        return new AddressDetailDto
                                        {
                                            MatchLevel = Convert.ToInt32(addressDetails[0]),
                                            Description = addressDetails[1].ToUpper(),
                                            Latitude = Convert.ToDouble(addressDetails[2]),
                                            Longitude = Convert.ToDouble(addressDetails[3]),
                                            ZipCode = addressDetails[4].Trim()
                                        };
                                    });
                if (addressess.Any(address => address.MatchLevel == 1))
                    addressess = addressess.Where(address => address.MatchLevel != 2);
                return addressess.ToList();

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public async Task SaveGeoRadiusSelection(SaveGeoRadiusDto input)
        {
            try {
                input.Selection.cValueOperator = "IN";
                input.Selection.iGroupOrder = 1;
                input.Selection.cJoinOperator = "AND";
                input.Selection.cValueMode = "T";
                input.Selection.cGrouping = "N";
                input.Selection.iGroupNumber = GetNewMaxGroupId(input.Selection.SegmentId);
                if(input.MatchLevel == 3)
                {
                    input.Selection.cQuestionFieldName = Global.ZipField;
                }
                input.Selection = CommonHelpers.ConvertNullStringToEmptyAndTrim(input.Selection);
                var segmentSelectionSaveDto = new SegmentSelectionSaveDto
                {
                    campaignId = input.CampaignId,
                    DatabaseId = input.DatabaseId,
                    selections = new List<SegmentSelectionDto> { input.Selection },
                    deletedSelections = new List<int>()
                };
                await CreateSegmentSelectionDetails(segmentSelectionSaveDto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task SaveGeoMappingSelection(SaveGeoRadiusDto input)
        {
            await SaveGeoRadiusSelection(input);
        }
        #endregion

        #region Multi-Field Search 
        public async Task SaveMultiFieldSelection(SegmentSelectionSaveDto selections)
        {
            var SegmentSelectionsList = new List<int>();
            try
            {
                var NewGroupID = GetNewMaxGroupId(selections.selections.FirstOrDefault().SegmentId);
                foreach (SegmentSelectionDto segSelection in selections.selections)
                {
                    segSelection.cCreatedBy = _mySession.IDMSUserName;
                    segSelection.iGroupNumber = NewGroupID;
                    SegmentSelectionsList.Add(_customSegmentSelectionRepository.AddSegmentSelection(segSelection, selections.campaignId));
                }
                GroupSelections(selections.selections[0].SegmentId, SegmentSelectionsList.Min(), SegmentSelectionsList);
                await _orderStatusManager.UpdateOrderStatus(selections.campaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }
}
