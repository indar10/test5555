using Abp.UI;
using Infogroup.IDMS.CampaignMaxPers.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace Infogroup.IDMS.Campaigns
{
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        #region MaxPer Tab
        private void ValidateMaXPerOnExceuteLink(int campaignId, bool isScheduled=false)
        {
            var maxPerValidStr = new StringBuilder();
            var isValidMaxPer = true;
            var listOfMaxPerGroupsSegments = _customCampaignMaxPerRepository.GetGroupsWithNotSegments(campaignId);
            var listOfUnDefinedGroupsForSegments = _customCampaignMaxPerRepository.GetUnDefinedGroupsForSegments(campaignId);
            maxPerValidStr.AppendLine("Campaign ID:" + campaignId);
            if (listOfMaxPerGroupsSegments != null && listOfMaxPerGroupsSegments.Count > 0)
            {
                isValidMaxPer = false;
                maxPerValidStr.AppendLine(L("InvalidMaxPerGroupsSegments"));
                var commaSeparatedDescriptions = string.Join(",", listOfMaxPerGroupsSegments.ToArray());
                maxPerValidStr.Append(L("MaxPerGroupList", commaSeparatedDescriptions));
            }
            if (listOfUnDefinedGroupsForSegments != null && listOfUnDefinedGroupsForSegments.Count > 0)
            {
                isValidMaxPer = false;
                maxPerValidStr.AppendLine(L("InvalidDefinedMaxPer"));
                var commaSeparatedDescriptions = string.Join(",", listOfUnDefinedGroupsForSegments.ToArray());
                maxPerValidStr.Append(L("MaxPerGroupList", commaSeparatedDescriptions));
            }
            maxPerValidStr.AppendLine(string.Empty);
            if (isScheduled)
            {
                maxPerValidStr.AppendLine(L("DontAllowForScheduleMessage"));
            }
            else
            {
                maxPerValidStr.AppendLine(L("DontAllowForExceutaionMessage"));
            }
            if (!isValidMaxPer)
                throw new UserFriendlyException(maxPerValidStr.ToString());
        }
        public List<SegmentLevelMaxPerDto> GetAllSegmentLevelMaxPer(int camapignID, List<DropdownOutputDto> listOfMaxPerFields)
        {
            var campaignOrderMaxPers = from maxPer in _campaignMaxPerRepository.GetAll()
                                       where maxPer.OrderId.Equals(camapignID) 
                                       select new SegmentLevelMaxPerDto()
                                       {
                                           cGroup = maxPer.cGroup,
                                           cMaxPerField = maxPer.cMaxPerField.ToUpper(),
                                           iMaxPerQuantity = maxPer.iMaxPerQuantity,
                                           dCreatedDate = maxPer.dCreatedDate,
                                           cCreatedBy = maxPer.cCreatedBy,
                                           dModifiedDate = maxPer.dModifiedDate,
                                           cModifiedBy = maxPer.cModifiedBy,
                                           Id = maxPer.Id,
                                           cMaxPerFieldDescription = listOfMaxPerFields.FirstOrDefault(t => t.Value.ToString().ToUpper()==(!string.IsNullOrEmpty(maxPer.cMaxPerField)? maxPer.cMaxPerField.ToUpper():string.Empty)).Label,
                                           SegmentLevelAction = ActionType.None
                                       };
            return campaignOrderMaxPers.ToList();
        }
        private GetCampaignMaxPerForViewDto FetchMaxPerData(Campaign campaign)
        {
            var emptyDropDownOutputDto = new DropdownOutputDto { Label = string.Empty, Value = string.Empty };
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var listOfMaxPerFields = _customCampaignMaxPerRepository.GetMaxPerFieldsDropdown(campaign.BuildID);
                var maxPerData = new GetCampaignMaxPerForViewDto
                {
                    GetSegmentLevelMaxPerData = GetAllSegmentLevelMaxPer(campaign.Id, listOfMaxPerFields),
                    GetMaxPerFieldDropdownData = new List<DropdownOutputDto> { emptyDropDownOutputDto },
                    GetCampaignLevelMaxPerData = new CampaignLevelMaxPerDto
                    {
                        cMinimumQuantity = campaign.iMinQuantityOrderLevelMaxPer,
                        cMaximumQuantity = campaign.iMaxQuantityOrderLevelMaxPer,
                        cMaxPerFieldOrderLevel = campaign.cMaxPerFieldOrderLevel
                    }
                };
                maxPerData.GetMaxPerFieldDropdownData.AddRange(listOfMaxPerFields);
                sw.Stop();
                Logger.Info($"\r\n ----- For campaignId:{campaign.Id}, Total time for FetchMaxPerData: {sw.Elapsed.TotalSeconds} ----- \r\n");
                return maxPerData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetMaxPerFieldDropdownData(int? databaseId, int? buildId, string Filter)
        {
            try
            {
                //Get the latest build only if user has MailerID else get BuildID from UI.
                if (buildId == null)
                    buildId = _buildAppService.GetLatestBuildFromDatabaseID(Convert.ToInt32(databaseId));
                var maxPerFields = _customCampaignMaxPerRepository.GetMaxPerFieldsDropdown(Convert.ToInt32(buildId));
                if (!string.IsNullOrWhiteSpace(Filter))
                    maxPerFields = maxPerFields.Where(e => e.Value.ToString().Contains(Filter, StringComparison.OrdinalIgnoreCase)).ToList();
                return maxPerFields;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        private void SaveMaxPerRecords(GetCampaignMaxPerForViewDto maxPerRecords, bool isOnlyCampaignLevel = false)
        {

            CheckMaxPerValidations(maxPerRecords, isOnlyCampaignLevel);
            if (!isOnlyCampaignLevel)
            {
                if (maxPerRecords.GetSegmentLevelMaxPerData != null && maxPerRecords.GetSegmentLevelMaxPerData.Count > 0)
                {
                    foreach (var updatedSegmentLevelMaxPerFields in maxPerRecords.GetSegmentLevelMaxPerData)
                    {

                        var updateSegLevelMaxPerRecord = _campaignMaxPerRepository.Get(updatedSegmentLevelMaxPerFields.Id);
                        updateSegLevelMaxPerRecord.cMaxPerField = updatedSegmentLevelMaxPerFields.cMaxPerField;
                        updateSegLevelMaxPerRecord.iMaxPerQuantity = updatedSegmentLevelMaxPerFields.iMaxPerQuantity!=null? updatedSegmentLevelMaxPerFields.iMaxPerQuantity:0;
                        updateSegLevelMaxPerRecord.dModifiedDate = DateTime.Now;
                        updateSegLevelMaxPerRecord.cModifiedBy = _mySession.IDMSUserName;
                        _campaignMaxPerRepository.UpdateAsync(updateSegLevelMaxPerRecord);

                    }
                }
            }
            if (maxPerRecords.GetCampaignLevelMaxPerData != null)
            {
                var campaign = _customCampaignRepository.Get(maxPerRecords.CampaignId);
                campaign.cMaxPerFieldOrderLevel = maxPerRecords.GetCampaignLevelMaxPerData.cMaxPerFieldOrderLevel;
                campaign.iMaxQuantityOrderLevelMaxPer = maxPerRecords.GetCampaignLevelMaxPerData.cMaximumQuantity;
                campaign.iMinQuantityOrderLevelMaxPer = maxPerRecords.GetCampaignLevelMaxPerData.cMinimumQuantity;
                campaign.cModifiedBy = _mySession.IDMSUserName;
                campaign.dModifiedDate = DateTime.Now;
            }

        }
        private void CheckMaxPerValidations(GetCampaignMaxPerForViewDto maxPerRecords, bool isOnlyCampaignLevel = false)
        {
            var validationErrors = new ArrayList();
            if (!isOnlyCampaignLevel)
            {
                var countOfZeroQuantity = maxPerRecords.GetSegmentLevelMaxPerData.Count(max => (max.iMaxPerQuantity.Equals(0) || max.iMaxPerQuantity == null) && !string.IsNullOrEmpty(max.cMaxPerFieldDescription));
                var countOfUnselectedMaxperfield = maxPerRecords.GetSegmentLevelMaxPerData.Count(max => (max.iMaxPerQuantity > 0) && string.IsNullOrEmpty(max.cMaxPerFieldDescription));

                if (countOfZeroQuantity > 0)
                    validationErrors.Add(L("InvalidZeroQuantitySegmentLevel"));
                if (countOfUnselectedMaxperfield > 0)
                    validationErrors.Add(L("InvalidUnSelectedMaxPerField"));
            }

            var minQty = maxPerRecords.GetCampaignLevelMaxPerData.cMinimumQuantity;
            var maxQty = maxPerRecords.GetCampaignLevelMaxPerData.cMaximumQuantity;
            var maxPerFieldValue = maxPerRecords.GetCampaignLevelMaxPerData.cMaxPerFieldOrderLevel;
            if (!string.IsNullOrEmpty(maxPerFieldValue) && (minQty.Equals(0) || maxQty.Equals(0)))
                validationErrors.Add(L("InvalidZeroQuantityCampaignLevel"));
            else if (minQty > maxQty)
                validationErrors.Add(L("MaxMinComparer"));

            if (validationErrors.Count > 0)
            {
                var count = 0;
                var builder = new System.Text.StringBuilder();
                if (validationErrors.Count.Equals(1))
                    builder.Append($"{validationErrors[0]}\n\n");
                else
                {
                    foreach (var item in validationErrors)
                        builder.Append($"{++count}: {item}\n\n");
                }

                throw new Exception(builder.ToString());
            }
        }
        #endregion
    }
}
