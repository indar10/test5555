using Abp.AspNetZeroCore.Net;
using Abp.UI;
using Infogroup.IDMS.Dto;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Campaigns
{
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        #region Print Summary Report       
        public async Task<FileDto> PrintDetailsReport(int iOrderID,int databaseID)
        {
            try
            {
                var query = _campaignBizness.GetDivisionIDFromOrderIDQuery(iOrderID);
                var fileName = $"CampaignSummary_{iOrderID}.xlsx";
                var divisionID = _customCampaignRepository.GetDivisionIDFromOrderID(query.Item1, query.Item2);
                var filePath = _idmsConfigurationCache.GetConfigurationValue("FileAttachmentPath", databaseID).cValue;
                var endpointaddress = _appConfiguration["Services:Uri"];
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var service = new IDMSCommonService.IDMSIQServiceClient(endpointaddress);
                await service.SaveSummaryReportAsync(iOrderID, filePath, fileName);
                stopwatch.Stop();
                var totalElapsedTime = stopwatch.Elapsed;
                var totalTime = $"{totalElapsedTime.Hours} hrs {totalElapsedTime.Minutes} mins {totalElapsedTime.Seconds} secs {totalElapsedTime.Milliseconds} ms";
                Logger.Info($"\r\n #PRINTSUMMARY | CampaignId:{iOrderID} | Total execution time: {totalTime} \r\n");
                var fileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet;               
                var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseID);
                if (awsFlag)
                {
                    var fileDownloadPath = _idmsConfigurationCache.GetConfigurationValue("FileAttachmentPathAWS", databaseID).cValue;
                    return new FileDto($"{fileDownloadPath}{fileName}", fileType, fileName, isAWS: awsFlag) ;
                }
                else
                {
                    return new FileDto($"{filePath}{fileName}", fileType, fileName);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        #endregion
    }
}
