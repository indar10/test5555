using Abp.AspNetZeroCore.Net;
using Abp.UI;
using Infogroup.IDMS.CampaignAttachments;
using Infogroup.IDMS.CampaignAttachments.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Shared.Dtos;
using Microsoft.AspNetCore.StaticFiles;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Infogroup.IDMS.Campaigns
{
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        #region Documents Tab
        public List<CampaignAttachmentDto> FetchAttachmentsData(int campaignID)
        {
            var campaignAttachments = new List<CampaignAttachmentDto>();
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                campaignAttachments = _customCampaignAttachmentsRepository.GetAttachmentsByID(campaignID);
                var lookupActiveAttachments = _lookupCache.GetLookUpFields("ORDERATTACHMENTTYPE");
                var list = new List<CampaignAttachmentDto>();
                foreach (var attachment in lookupActiveAttachments)
                {
                    list = campaignAttachments.Where(c => c.Code.Equals(attachment.cCode.Trim())).ToList();
                    if (list.Count() <= 0)
                    {
                        var dto = new CampaignAttachmentDto
                        {
                            FormType = attachment.cDescription,
                            Code = attachment.cCode,
                            OrderId = campaignID.ToString(),
                            ID = 0,
                            cFileName = string.Empty,
                            RealFileName = string.Empty,
                            Action = ActionType.None,
                            IsDisabled = false
                        };
                        campaignAttachments.Add(dto);
                    }
                }
                sw.Stop();
                Logger.Info($"\r\n ----- For campaignId:{campaignID}, Total time for FetchAttachmentsData: {sw.Elapsed.TotalSeconds} ----- \r\n");
                return campaignAttachments;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public void DeleteCampaignAttachment(int id)
        {
            try
            {
                if (id > 0)
                {
                    _customCampaignAttachmentsRepository.DeleteAsync(id);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public void UploadFile(string filename, string sysfileName, string code, int id, int campaignId)
        {

            try
            {
                if (id == 0)
                {

                    var camapignAttachment = new CampaignAttachment
                    {

                        cCreatedBy = _mySession.IDMSUserName,
                        dCreatedDate = DateTime.Now,
                        LK_AttachmentType = code.ToUpper(),
                        cModifiedBy = null,
                        dModifiedDate = null,
                        cFileName = filename,
                        cRealFileName = sysfileName,
                        OrderId = campaignId
                    };
                    _customCampaignAttachmentsRepository.InsertAsync(camapignAttachment);
                }
                else
                {
                    var editCampaignAttachment = _customCampaignAttachmentsRepository.Get(id);
                    editCampaignAttachment.LK_AttachmentType = code.ToUpper();
                    editCampaignAttachment.cModifiedBy = _mySession.IDMSUserName;
                    editCampaignAttachment.dModifiedDate = DateTime.Now;
                    editCampaignAttachment.cFileName = filename;
                    editCampaignAttachment.cRealFileName = sysfileName;
                    editCampaignAttachment.OrderId = campaignId;
                    _customCampaignAttachmentsRepository.UpdateAsync(editCampaignAttachment);

                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public FileDto DownloadFile(int campaignId, string attachmentType, int databaseId)
        {
            try
            {
                var documentRecord = _customCampaignAttachmentsRepository.GetAll().FirstOrDefault(x => x.OrderId == campaignId && x.LK_AttachmentType == attachmentType);
                var fileName = documentRecord.cRealFileName;
                var downloadedFileName = documentRecord.cFileName;
                var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseId);
                var filePath = _idmsConfigurationCache.GetConfigurationValue("ORDER_ATTACHMENT_PATH", databaseId)?.cValue;
                if (awsFlag)
                    filePath = filePath.Trim('/') + "/";
                else
                    filePath += @"\";
                string contentType;
                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
                //var fileType = MimeTypeNames.ApplicationPdf;
                return new FileDto($"{filePath}{fileName}", contentType, downloadedFileName, isAWS: awsFlag);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        #endregion
    }
}
