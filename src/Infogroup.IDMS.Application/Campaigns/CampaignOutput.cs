using Abp.Authorization;
using Abp.UI;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.CampaignFTPs;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.OrderExportParts.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Infogroup.IDMS.Campaigns
{
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        #region OutPut Tab
        private GetCampaignsOutputDto FetchOutputDetails(Campaign campaign, int divisionId, int currentStatus)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var campaignId = campaign.Id;
                var campaignFtp = _campaignFTPRepository.GetAll().FirstOrDefault(x => x.OrderID == campaignId);
                var getCampaignOutput = new GetCampaignsOutputDto
                {
                    Type = string.IsNullOrEmpty(campaign.LK_ExportFileFormatID) ? string.Empty : campaign.LK_ExportFileFormatID,
                    IsHeaderRow = campaign.iIsAddHeader,
                    IsDataFileOnly = campaign.iIsExportDataFileOnly,
                    IsUnzipped = campaign.iIsUncompressed,
                    Sort = string.IsNullOrEmpty(campaign.cSortFields) ? string.Empty : campaign.cSortFields,
                    PGPKey = string.IsNullOrEmpty(campaign.LK_PGPKeyFile) ? string.Empty : campaign.LK_PGPKeyFile,
                    ShipTo = campaign.cShipTOEmail,
                    FTPSite = campaignFtp == null ? string.Empty : campaignFtp.cFTPServer,
                    UserName = campaignFtp == null ? string.Empty : campaignFtp.cUserID,
                    LayoutId = campaign.iExportLayoutID.ToString(),
                    Layoutlist = GetCampaignOutputDDValues(campaignId, 2),
                    SortList = GetCampaignOutputDDValues(campaignId, 1),
                    TypeList = GetCampaignOutputDDValues(campaignId, 5),
                    PGPKeyList = GetCampaignOutputDDValues(campaignId, 4),
                    ShipToList = GetCampaignOutputDDValues(campaignId, 3, divisionId),
                    ShipCCEmail = string.IsNullOrEmpty(campaign.cShipCCEmail) ? string.Empty : campaign.cShipCCEmail,
                    ShipSubject = string.IsNullOrEmpty(campaign.cShipSUBJECT) ? string.Empty : campaign.cShipSUBJECT,
                    ShipNotes = string.IsNullOrEmpty(campaign.cNotes) ? string.Empty : campaign.cNotes,
                    ShippedDate = campaign.dShipDateShipped.ToString(),
                    SplitType = campaign.iSplitType,
                    SplitIntoNParts = campaign.iSplitIntoNParts,
                    MediaList = GetCampaignOutputDDValues(campaignId, 6),
                    Media = campaign.LK_Media,
                    FileLabel = campaign.cFileLabel,
                    FileNotes = campaign.cSpecialProcess,
                    TotalOutputQuantity = GetTotalOutputQuantity(campaign, currentStatus == 40)
                };

                if ((string.IsNullOrEmpty(campaign.iExportLayoutID.ToString()) || campaign.iExportLayoutID == 0) && !string.IsNullOrEmpty(campaign.cExportLayout))
                {
                    campaign.iExportLayoutID = 0;
                    getCampaignOutput.Layoutlist.Insert(0, new DropdownOutputDto { Label = $"0 : {campaign.cExportLayout}", Value = campaign.iExportLayoutID.ToString() });
                    getCampaignOutput.Layout = "0";
                }
                else
                {
                    getCampaignOutput.Layout = string.IsNullOrEmpty(campaign.cExportLayout)?string.Empty:campaign.iExportLayoutID.ToString();
                }
                sw.Stop();
                Logger.Info($"\r\n ----- For campaignId:{campaignId}, Total time for FetchOutputDetails: {sw.Elapsed.TotalSeconds} ----- \r\n");
                return getCampaignOutput;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private List<DropdownOutputDto> GetCampaignOutputDDValues(int campaignId, int type, int divisionId=0)
        {
            try
            {
                List<DropdownOutputDto> dropdownValues = null;
                switch (type)
                {
                    case 1: // For SortValues
                        dropdownValues = _customCampaignRepository.GetCampaignSortValue(campaignId);
                        break;
                    case 2: // For OutputLayouts
                        var outputLayouts = _customCampaignRepository.GetAllOutputLayouts(campaignId);
                        dropdownValues = outputLayouts.Select(seg => new DropdownOutputDto()
                        {
                            Value = seg.Id.ToString(),
                            Label = seg.Label
                        }).ToList();
                        break;
                    case 3: //For ShipToValues
                        var queryShipTo = _campaignBizness.GetCampaignShipToValues(divisionId);
                        var outputShipTo = _customCampaignRepository.GetCampaignShipToValues(queryShipTo.Item1, queryShipTo.Item2);
                        dropdownValues = outputShipTo.Select(seg => new DropdownOutputDto()
                        {
                            Value = seg.Id.ToString(),
                            Label = seg.Label
                        }).ToList();
                        break;
                    case 4: //For PgpKeyValues
                        var lstLookUpForPGP = _lookupCache.GetLookUpFields("PGPKEY").OrderBy(x => x.cDescription);
                        dropdownValues = lstLookUpForPGP.Select(x => new DropdownOutputDto()
                        {
                            Value = x.cCode,
                            Label = x.cDescription
                        }).ToList();
                        break;
                    case 5: // For TypeValues
                        var lstLookUpForType = _lookupCache.GetLookUpFields("EXPORTFILEFORMAT").OrderBy(x => x.cDescription);
                        dropdownValues = lstLookUpForType.Select(x => new DropdownOutputDto()
                        {
                            Value = x.cCode,
                            Label = x.cDescription
                        }).ToList();
                        break;
                    case 6: //For Media Values
                        var lstLookupForMedia = _lookupCache.GetLookUpFields("OUTPUTMEDIA");
                        dropdownValues = lstLookupForMedia.Select(x => new DropdownOutputDto()
                        {
                            Value = x.cCode,
                            Label = x.cDescription
                        }).ToList();
                        break;
                    default:
                        break;
                }
                return dropdownValues;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public GetCampaignsOutputDto GetDetailsByCompanyIdAndOrderId(int campaignId, int companyId)
        {
            try
            {
                var query = _campaignBizness.GetDivisionIDFromOrderIDQuery(campaignId);
                var divisionId = _customCampaignRepository.GetDivisionIDFromOrderID(query.Item1, query.Item2);
                var queryFtpDetails = _campaignBizness.GetFtpDetailsByCompanyIdAndDivisionrId(companyId, divisionId);
                var ftpDetails = _customCampaignRepository.GetFtpDetailsByCompanyIdAndDivisionrId(queryFtpDetails.Item1, queryFtpDetails.Item2);
                return ftpDetails;


            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public FileDto DownloadOutputLayoutTemplateExcelTest(int campaignId)
        {
            try
            {
                var buildId = _campaignRepository.Get(campaignId).BuildID;
                var layoutTemplateList = _customCampaignRepository.GetOutputLayoutTemplate(buildId, campaignId);
                return _layoutTemplateExcelExporter.ExportToFile(layoutTemplateList);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public Campaign GetSplitPartForCampaignId(int campaignId)
        {
            try
            {
                return _customCampaignRepository.Get(campaignId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private void UpdateCampaignFTPDetails(EditCampaignsOutputDto input, int campaignId)
        {
            try
            {
                var campaignFtp = _campaignFTPRepository.GetAll().FirstOrDefault(x => x.OrderID == campaignId);
                var query = _campaignBizness.GetDivisionIDFromOrderIDQuery(campaignId);
                var divisionId = _customCampaignRepository.GetDivisionIDFromOrderID(query.Item1, query.Item2);
                var queryFtpDetails = _campaignBizness.GetFtpDetailsByCompanyIdAndDivisionrId(input.CompanyId, divisionId);
                var ftpDetails = _customCampaignRepository.GetFtpDetailsByCompanyIdAndDivisionrId(queryFtpDetails.Item1, queryFtpDetails.Item2);
                if (campaignFtp != null)
                {
                    if (input.CompanyId != 0)
                    {
                        if (input.FTPSite.Trim().Length == 0)                        
                            _campaignFTPRepository.Delete(campaignFtp.Id);                        
                        else
                        {
                            campaignFtp.cFTPServer = input.FTPSite;
                            campaignFtp.cUserID = input.UserName;
                            campaignFtp.cPassword = ftpDetails.FTPPassword;
                            campaignFtp.cModifiedBy = _mySession.IDMSUserName;
                            campaignFtp.dModifiedDate = DateTime.Now;
                            _campaignFTPRepository.Update(campaignFtp);
                            CurrentUnitOfWork.SaveChanges();
                        }                        
                    }
                }
                else
                {
                    if (input.CompanyId != 0 && !string.IsNullOrEmpty(input.UserName))
                    {
                        var campaignFtpObject = new CampaignFTP
                        {
                            OrderID = campaignId,
                            cFTPServer = input.FTPSite,
                            cUserID = input.UserName,
                            cPassword = ftpDetails.FTPPassword,
                            cCreatedBy = _mySession.IDMSUserName,
                            dCreatedDate = DateTime.Now
                        };
                        _campaignFTPRepository.Insert(campaignFtpObject);
                        CurrentUnitOfWork.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<CampaignExportPartDto> GetExportParts(int campaignId, int part)
        {
            try
            {
                return _customCampaignRepository.GetExportPartsSelection(campaignId, part);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }
}
