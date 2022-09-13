using Abp.Authorization;
using Abp.UI;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.OrderStatuss;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Campaigns
{
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        #region CampaignActions
        public async Task<CampaignActionOutputDto> CampaignActions(CampaignActionInputDto input)
        {
            try
            {
                var result = new CampaignActionOutputDto { Success = true };
                // Status Change Validation
                var changeStatusValidation = ValidateChangeStatus(input.CampaignId, input.CampaignStatus);
                if (!changeStatusValidation.Success) throw new UserFriendlyException(changeStatusValidation.Message);
                switch ((CampaignStatus)input.CampaignStatus)
                {
                    case CampaignStatus.OrderCreated: // Exceute
                    case CampaignStatus.OrderFailed:
                        ValidateMaXPerOnExceuteLink(input.CampaignId);
                        ValidateBuild(input.BuildId);
                        ValidateOutputFields(input.CampaignId, "selection", input.DatabaseId);
                        await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.OrderSubmitted, _mySession.IDMSUserName, input.CNotes);
                        result.Message = L("Campaign_Submit_Count", input.CampaignId);
                        break;
                    case CampaignStatus.OrderSubmitted: // Cancel Execution
                        var campaignObject = _campaignRepository.FirstOrDefault(o => o.Id == input.CampaignId);
                        campaignObject.iIsOrder = false;
                        campaignObject.cModifiedBy = _mySession.IDMSUserName;
                        campaignObject.dModifiedDate = DateTime.Now;
                        await _campaignRepository.UpdateAsync(campaignObject);
                        await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                        result.Message = L("Campaign_Cancel", input.CampaignId);
                        break;
                    case CampaignStatus.OrderCompleted:// Output
                        if (input.IsExecute == true)
                        {
                            ValidateMaXPerOnExceuteLink(input.CampaignId);
                            ValidateBuild(input.BuildId);
                            ValidateOutputFields(input.CampaignId, "selection", input.DatabaseId);
                            await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.OrderSubmitted, _mySession.IDMSUserName);
                            result.Message = L("Campaign_Submit_Count", input.CampaignId);
                        }
                        else if (VerifyCountInputs(input.CampaignId))
                        {
                            var campaign = _campaignRepository.Get(input.CampaignId);
                            if (campaign.LK_ExportFileFormatID.Trim() == "EE")
                            {
                                var totalCount = GetTotalOutputQuantity(campaign, true);
                                if (totalCount > 1000000)                                
                                    throw new UserFriendlyException(L("ExportFileFormatError"));                                
                            }
                            if (string.IsNullOrWhiteSpace(campaign.cShipTOEmail)) throw new UserFriendlyException(L("shipToNotAvailable"));
                            if (string.IsNullOrWhiteSpace(campaign.cExportLayout)) throw new UserFriendlyException(L("exportLayoutNotAvailable"));
                            if (PermissionChecker.IsGranted(AppPermissions.Pages_Campaigns_Billing) && string.IsNullOrWhiteSpace(campaign.cLVAOrderNo))
                                throw new UserFriendlyException(L("PONotAvailable"));
                            if (!CheckFileExists(input.CampaignId))
                                throw new UserFriendlyException(L("OutputFileAlreadyExists"));
                            if (!(CheckSortFieldExists(input.CampaignId)))
                                throw new UserFriendlyException(L("SortFieldException"));
                            await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.OutputSubmitted, _mySession.IDMSUserName);
                            result.Message = L("Campaign_Submit_Output", input.CampaignId);
                        }
                        break;
                    case CampaignStatus.OutputSubmitted: // Cancel Output
                        await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.OrderCompleted, _mySession.IDMSUserName);
                        result.Message = L("Campaign_Cancel", input.CampaignId);
                        break;
                    case CampaignStatus.OutputCompleted: // Ship                         
                        if (!_permissionChecker.IsGranted(_mySession.IDMSUserId, input.DatabaseId, PermissionList.OrderShippingApproval, AccessLevel.iAddEdit))
                            throw new UserFriendlyException(L("ShipDeniedForDB"));
                        var msg = CheckOrderAttachment(input.CampaignId);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            throw new UserFriendlyException(msg);
                        }
                        var oessStatus = _customCampaignRepository.GetOESSStatusByCampaignID(input.CampaignId);
                        if (oessStatus > 0 && oessStatus != (int)CampaignOESSConsts.ApprovedByCredit && oessStatus != (int)CampaignOESSConsts.Invoiced) throw new UserFriendlyException(L("OESSShippingValidation"));
                        if (!ValidateMailerBroker(input.CampaignId)) throw new UserFriendlyException(L("MailerBrokerValidationMsg"));
                        await _orderStatusManager.UpdateOrderStatus(input.CampaignId, CampaignStatus.ApprovedforShipping, _mySession.IDMSUserName);
                        result.Message = L("Campaign_Submit_Ship", input.CampaignId);
                        break;
                    default:
                        break;
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public void UpdateOrderStatus(CampaignActionInputDto input)
        {
            try
            {
                _orderStatusManager.UpdateOrderStatus(input.CampaignId, (CampaignStatus)input.CampaignStatus, _mySession.IDMSUserName, input.CNotes);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private int GetTotalOutputQuantity(Campaign campaign, bool status = false)
        {
            try
            {
                var totalCount = 0;
                var decoyCount = 0;
                if(status)
                    decoyCount = _campaignDecoyRepository.Count(x => x.OrderId == campaign.Id);                
                var segment = _segmentRepository.GetAll().Where(x => x.OrderId == campaign.Id).
                    Select(x => new { x.iOutputQty, x.iProvidedQty} ).ToList();
                foreach (var item in segment)
                {
                    if (item.iOutputQty == -1)
                        totalCount += item.iProvidedQty;
                    else
                        totalCount += (int)item.iOutputQty;
                }                
                return totalCount + decoyCount;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }            
        }

        public async Task<CampaignActionOutputDto> ExecuteCampaign(CampaignActionInputDto input)
        {
            try
            {
                var validateLatestBuild = CheckLatestBuild(input.DatabaseId, input.CurrentBuild);
                if (!validateLatestBuild.Success) return validateLatestBuild;
                else return await CampaignActions(input);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        private CampaignActionOutputDto CheckLatestBuild(int databaseId, int currentBuild)
        {
            try
            {
                var result = new CampaignActionOutputDto { Success = true };
                var allBuilds = _buildRepository.GetAll();
                string cBuild = allBuilds.Where(build => build.DatabaseId == databaseId && build.iIsReadyToUse && build.iIsOnDisk).OrderByDescending(build => build.Id).FirstOrDefault()?.cBuild ?? "0";
                int.TryParse(cBuild, out int latestBuild);
                if (latestBuild > currentBuild)
                {
                    string currentBuildDescription = allBuilds.Where(build => build.DatabaseId == databaseId && string.Equals(build.cBuild, currentBuild.ToString()) ).FirstOrDefault()?.cDescription ?? "0";
                    string latestBuildDescription = allBuilds.Where(build => build.DatabaseId == databaseId && string.Equals(build.cBuild, latestBuild.ToString())).FirstOrDefault()?.cDescription ?? "0";
                    result.Success = false;
                    result.Message = L("NewBuildAvailable", latestBuildDescription, currentBuildDescription);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public void CampaignScheduleActionsValidations(CampaignActionInputDto input)
        {
            try
            {
                var result = new CampaignActionOutputDto { Success = true };
                switch ((CampaignStatus)input.CampaignStatus)
                {
                    case CampaignStatus.OrderCreated:
                        ValidateOutputFields(input.CampaignId, "selection", input.DatabaseId);
                        break;

                    case CampaignStatus.OrderCompleted:
                        if (VerifyCountInputs(input.CampaignId))
                        {
                            var campaign = _campaignRepository.Get(input.CampaignId);
                            if (campaign.LK_ExportFileFormatID.Trim() == "EE")
                            {
                                var totalCount = GetTotalOutputQuantity(campaign, true);
                                if (totalCount > 1000000)
                                    throw new UserFriendlyException(L("ExportFileFormatError"));
                            }
                            if (string.IsNullOrWhiteSpace(campaign.cShipTOEmail)) throw new UserFriendlyException(L("shipToNotAvailable"));
                            if (string.IsNullOrWhiteSpace(campaign.cExportLayout)) throw new UserFriendlyException(L("exportLayoutNotAvailable"));
                            if (PermissionChecker.IsGranted(AppPermissions.Pages_Campaigns_Billing) && string.IsNullOrWhiteSpace(campaign.cLVAOrderNo))
                                throw new UserFriendlyException(L("PONotAvailable"));
                            if (!CheckFileExists(input.CampaignId))
                                throw new UserFriendlyException(L("OutputFileAlreadyExists"));
                            if (!(CheckSortFieldExists(input.CampaignId)))
                                throw new UserFriendlyException(L("SortFieldException"));
                        }
                        break;

                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public CampaignActionOutputDto CheckLatestBuildForScheduleCampaign(CampaignActionInputDto input)
        {
            try
            {
                return CheckLatestBuild(input.DatabaseId, input.CurrentBuild);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public void ScheduleCampaign(int campaignId, int newStatus, string date, string time, int buildId, int currentStatus)
        {
            try
            {
                var changeStatusValidation = ValidateChangeStatus(campaignId, currentStatus);
                if (!changeStatusValidation.Success) throw new UserFriendlyException(changeStatusValidation.Message);
                ValidateBuild(buildId);
                if (currentStatus == 10)
                {
                    ValidateMaXPerOnExceuteLink(campaignId,true);
                }
                _orderStatusManager.UpdateScheduleStatus(campaignId, newStatus, _mySession.IDMSUserName, time, date);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Campaigns_Reship)]
        public async Task ReshipCampaign(int campaignId, int databaseId, string reshipEmail)
        {
            try
            {
                if (!_permissionChecker.IsGranted(_mySession.IDMSUserId, databaseId, PermissionList.ReshipOrder, AccessLevel.iAddEdit))
                    throw new UserFriendlyException(L("ReshipDeniedForDB"));
                if (!string.IsNullOrEmpty(reshipEmail))
                    reshipEmail = reshipEmail.Trim();
                var campaignObject = _campaignRepository.Get(campaignId);
                if (!string.IsNullOrEmpty(campaignObject.cShipTOEmail) && !string.IsNullOrEmpty(reshipEmail))
                {
                    campaignObject.cShipTOEmail = $"{campaignObject.cShipTOEmail};{reshipEmail.Trim()}";
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                await _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.ApprovedforShipping, _mySession.IDMSUserName);
            }

            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Campaigns_Cancel)]
        public async Task CancelCampaign(int campaignId)
        {
            try
            {
                await _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.Cancelled, _mySession.IDMSUserName);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Campaigns_Reset)]
        public async Task ResetCampaign(int campaignId)
        {
            try
            {
                await _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.OutputCompleted, _mySession.IDMSUserName);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        private bool ValidateMailerBroker(int campaignId)
        {
            var databaseId = _databaseRepository.GetDataSetDatabaseByOrderID(campaignId).Id;
            if (GetDivisionMailerBroker(databaseId))
            {
                var campaignObject = _campaignRepository.Get(campaignId);
                if (campaignObject.DivisionBrokerID == 0 || campaignObject.DivisionMailerID == 0) return false;
            }
            return true;
        }

        private bool CheckFileExists(int campaignId)
        {
            var fileExists = _customCampaignRepository.CheckIfOutputFileExists(campaignId);
            return fileExists;
        }

        private CampaignActionOutputDto ValidateChangeStatus(int campaignID, int campaignStatus)
        {
            var result = new CampaignActionOutputDto { Success = true };
            var currentStatusObject = _orderStatusRepository.FirstOrDefault(o => o.OrderID == campaignID && o.iIsCurrent);
            if (campaignStatus != currentStatusObject.iStatus)
            {
                result.Success = false;
                var errMsg = currentStatusObject.iStatus == 0 ? L("campaignDeleted") : L("campaignStatusChanged");
                result.Message = $"Campaign ID {campaignID} {Environment.NewLine}{errMsg}";
            }
            return result;
        }

        private bool ValidateBuild(int buildId)
        {
            try
            {
                bool validBuild = true;
                var campaignBuild = _buildRepository.Get(buildId);
                //Check for special user have permission to allow counts to be run on inactive build
                bool runCountsOnInactiveBuild = _permissionChecker.IsGranted(_mySession.IDMSUserId, PermissionList.RunCountsInactiveBuild, AccessLevel.iAddEdit);
                if (runCountsOnInactiveBuild && campaignBuild != null && campaignBuild.Id > 0 && campaignBuild.iIsReadyToUse)
                {
                    //Allow special user even if build is no longer available.
                    validBuild = true;
                }
                else if (campaignBuild != null && campaignBuild.Id > 0 && !campaignBuild.iIsOnDisk)
                {
                    throw new UserFriendlyException(L("InactiveBuildValidation"));
                }
                return validBuild;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        private bool CheckSortFieldExists(int campaignId)
        {
            var campaign = _customCampaignRepository.Get(campaignId);
            var sortFieldForOrder = !string.IsNullOrEmpty(campaign.cSortFields) ? campaign.cSortFields : "";
            var mediaType = !string.IsNullOrEmpty(campaign.LK_Media) ? campaign.LK_Media : "";
            if (mediaType.ToUpper().Equals("E"))
            {
                if (!string.IsNullOrEmpty(sortFieldForOrder))
                {
                    if (!sortFieldForOrder.ToUpper().Equals("KEYCODE1"))
                    {
                        var exportLayoutDetails = _campaignExportLayoutRepository.GetAll().Where(x => x.OrderId == campaignId).ToList();
                        var countForSortField = exportLayoutDetails.Where(x => x.cFieldName.ToUpper() == sortFieldForOrder.ToUpper()).ToList().Count();
                        return countForSortField <= 0 ? false : true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private bool VerifyCountInputs(int campaignId)
        {
            var bolReturnValue = false;
            var endpointaddress = _appConfiguration["Services:Uri"];
            var service = new IDMSCommonService.IDMSIQServiceClient(endpointaddress);
            var verifyCountInputs = service.VerifyCountInputsAsync(campaignId);
            var validationMsg = string.Empty;
            if (!string.IsNullOrEmpty(verifyCountInputs.Result.VerifyCountInputsResult.LogMessages))
                validationMsg = verifyCountInputs.Result.VerifyCountInputsResult.LogMessages.Replace("\r\n", "\n").Replace("'", "");
            if (verifyCountInputs.Result.VerifyCountInputsResult.IsVerifyCountInputs)
                bolReturnValue = true;
            else
            {
                throw new UserFriendlyException(validationMsg);
            }
            return bolReturnValue;
        }

        private string CheckOrderAttachment(int campaignId)
        {

            var validationMsg = string.Empty;

            try
            {
                var buildId = _campaignRepository.GetAll().Where(p => p.Id == campaignId).FirstOrDefault().BuildID;
                var databaseId = _buildRepository.GetAll().Where(p => p.Id == buildId).FirstOrDefault().DatabaseId;

                var LVOQty = _idmsConfigurationCache.GetConfigurationValue("LVO_MAX_QUANTITY", Convert.ToInt32(databaseId)).iValue;
                var LVORequired = _idmsConfigurationCache.GetConfigurationValue("LVO_FORM_REQUIRED", Convert.ToInt32(databaseId)).cValue.Trim() == "1" ? true : false;
                var FAXRequired = _idmsConfigurationCache.GetConfigurationValue("FAX_FORM_REQUIRED", Convert.ToInt32(databaseId)).cValue.Trim() == "1" ? true : false;

                if (LVORequired || FAXRequired)
                {
                    var campaign = _campaignRepository.GetAll().Where(p => p.Id == campaignId).FirstOrDefault();
                    var campaignAttachment = _customCampaignAttachmentsRepository.GetAll().Where(p => p.OrderId == campaignId).ToList();

                    if (LVORequired && (LVOQty > 0) && (campaign.iProvidedCount > LVOQty) && !campaignAttachment.Where(p => p.LK_AttachmentType == "LVO").Any())
                    {
                        validationMsg += Environment.NewLine + L("UploadLVO");
                    }
                    if (FAXRequired && campaign.cChannelType == "T" && !campaignAttachment.Where(p => p.LK_AttachmentType == "FWF").Any())
                    {
                        validationMsg += Environment.NewLine + L("UploadFaxWaiver");
                    }
                }
            }
            catch (Exception)
            {
                validationMsg = L("OrderAttachmentValidation");
            }
            return validationMsg;

        }
        #endregion

        public void ValidateOutputFields(int campaignId, string validationString, int databaseId)
        {
            var isAWS = _idmsConfigurationCache.IsAWSConfigured(databaseId);
            if (!isAWS)
            {
                var endpointaddress = _appConfiguration["Services:Uri"];
                var service = new IDMSCommonService.IDMSIQServiceClient(endpointaddress);
                var strMissingFields = service.ValidateOutputFieldsAsync(campaignId, validationString);
                var validationMsg = string.Empty;
                if (!string.IsNullOrEmpty(strMissingFields.Result.ValidateOutputFieldsResult))
                {
                    if ((strMissingFields.Result.ValidateOutputFieldsResult.Contains("archived.") || strMissingFields.Result.ValidateOutputFieldsResult.Contains("Uploaded file:")) && isAWS)
                        return;

                    if (strMissingFields.Result.ValidateOutputFieldsResult.Contains("Previous Order(s)"))
                        strMissingFields.Result.ValidateOutputFieldsResult = strMissingFields.Result.ValidateOutputFieldsResult.Replace("Previous Order(s)", "Campaign History campaign(s)");
                    else if (strMissingFields.Result.ValidateOutputFieldsResult.Contains("order"))
                        strMissingFields.Result.ValidateOutputFieldsResult = strMissingFields.Result.ValidateOutputFieldsResult.Replace("order", "campaign");

                    validationMsg = strMissingFields.Result.ValidateOutputFieldsResult;
                    throw new UserFriendlyException(validationMsg);
                }
            }
        }

        public string GetServerDate()
        {
            return DateTime.Now.Date.ToShortDateString();
        }
    }
}
