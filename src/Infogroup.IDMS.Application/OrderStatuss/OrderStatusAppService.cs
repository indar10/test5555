using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Sessions;
using System.Collections.Generic;
using Infogroup.IDMS.OrderStatuss.Dtos;
using Infogroup.IDMS.IDMSUsers;
using Abp.UI;
using Abp.Authorization;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.IDMSConfigurations;
using System.IO;
using Infogroup.IDMS.Segments;
using System.Text;
using Castle.Core.Logging;

namespace Infogroup.IDMS.OrderStatuss
{
    [AbpAuthorize]
    public class OrderStatusAppService : IDMSAppServiceBase, IOrderStatusAppService
    {
        private readonly IRepository<OrderStatus> _orderStatusRepository;
        private readonly AppSession _mySession;
        private readonly IRepository<IDMSUser, int> _userRepository;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IRepository<Segment, int> _segmentRepository;

        public OrderStatusAppService(
            IRepository<OrderStatus> orderStatusRepository,
            AppSession mySession,
            IRepository<IDMSUser, int> userRepository,
            IDatabaseRepository databaseRepository,
            IRedisIDMSConfigurationCache idmsConfigurationCache,
            IRepository<Segment, int> segmentRepository
            )
        {
            _orderStatusRepository = orderStatusRepository;
            _mySession = mySession;
            _userRepository = userRepository;
            _databaseRepository = databaseRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
            _segmentRepository = segmentRepository;
        }

        #region Get Campaign Status History
        public List<CampaignStatusDto> GetStatusHistory(int campaignId)
        {
            return (from campaignStatus in _orderStatusRepository.GetAll()
                    join userDetails in _userRepository.GetAll()
                    on campaignStatus.cCreatedBy.Trim().ToUpper()
                    equals userDetails.cUserID.Trim().ToUpper()
                    where campaignStatus.OrderID == campaignId
                    orderby campaignStatus.Id descending
                    select new CampaignStatusDto
                    {
                        Id = campaignStatus.Id,
                        iStopRequested = campaignStatus.iStopRequested != null && Convert.ToBoolean(campaignStatus.iStopRequested) ? $"Yes ({campaignStatus.cModifiedBy}, {Convert.ToDateTime(campaignStatus.dModifiedDate).ToString("MM/dd/yyyy")})" : "No",
                        iStatus = campaignStatus.iStatus,
                        cCreatedBy = !string.IsNullOrEmpty(userDetails.cFirstName) && !string.IsNullOrEmpty(userDetails.cLastName) ? $"{userDetails.cFirstName} {userDetails.cLastName}" : campaignStatus.cCreatedBy,
                        dCreatedDate = string.Format("{0:MMM dd yyyy hh:mm:sstt}", campaignStatus.dCreatedDate),
                    }).ToList();
        }
        #endregion

        #region Update Campaign Status 
        public async Task UpdateCampaignStatus(int campaignID)
        {
            try
            {
                var currentStatusObject = _orderStatusRepository.FirstOrDefault(o => o.OrderID == campaignID && o.iIsCurrent);

                if (((CampaignStatus)currentStatusObject.iStatus == CampaignStatus.OrderRunning) ||
                    ((CampaignStatus)currentStatusObject.iStatus == CampaignStatus.OutputRunning) ||
                    ((CampaignStatus)currentStatusObject.iStatus == CampaignStatus.WaitingtoShip))
                {
                    currentStatusObject.dModifiedDate = DateTime.Now;
                    currentStatusObject.cModifiedBy = _mySession.IDMSUserName;
                    currentStatusObject.iStopRequested = true;
                    await _orderStatusRepository.UpdateAsync(currentStatusObject);
                    CurrentUnitOfWork.SaveChanges();
                }
                else if (((CampaignStatus)currentStatusObject.iStatus == CampaignStatus.OrderSubmitted) ||
                    ((CampaignStatus)currentStatusObject.iStatus == CampaignStatus.OutputSubmitted) ||
                    ((CampaignStatus)currentStatusObject.iStatus == CampaignStatus.ApprovedforShipping))
                {
                    currentStatusObject.dModifiedDate = DateTime.Now;
                    currentStatusObject.cModifiedBy = _mySession.IDMSUserName;
                    currentStatusObject.iStopRequested = true;
                    currentStatusObject.iIsCurrent = false;

                    await _orderStatusRepository.UpdateAsync(currentStatusObject);
                    CurrentUnitOfWork.SaveChanges();

                    var orderStatus = new OrderStatus();

                    switch ((CampaignStatus)currentStatusObject.iStatus)
                    {
                        case CampaignStatus.OrderSubmitted:
                            orderStatus.iStatus = Convert.ToInt32(CampaignStatus.OrderFailed);
                            break;

                        case CampaignStatus.OutputSubmitted:
                            orderStatus.iStatus = Convert.ToInt32(CampaignStatus.OutputFailed);
                            break;

                        case CampaignStatus.ApprovedforShipping:
                            orderStatus.iStatus = Convert.ToInt32(CampaignStatus.ShippingFailed);
                            break;

                        default:
                            break;
                    }

                    orderStatus.OrderID = campaignID;
                    orderStatus.iIsCurrent = true;
                    orderStatus.cNotes = currentStatusObject.cNotes;
                    orderStatus.cCreatedBy = _mySession.IDMSUserName;
                    orderStatus.dCreatedDate = DateTime.Now;

                    await _orderStatusRepository.InsertAsync(orderStatus);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    switch ((CampaignStatus)currentStatusObject.iStatus)
                    {
                        case (CampaignStatus.OrderCompleted):
                            throw new UserFriendlyException(L("CampaignCompletedCancel"));

                        case (CampaignStatus.OrderFailed):
                            throw new UserFriendlyException(L("CampaignFailedCancel"));

                        case (CampaignStatus.ReadytoOutput):
                            throw new UserFriendlyException(L("CampaignReadyOutputCancel"));

                        case (CampaignStatus.OutputCompleted):
                            throw new UserFriendlyException(L("CampaignOutputCancel"));

                        case (CampaignStatus.OutputFailed):
                            throw new UserFriendlyException(L("CampaignOutputFailCancel"));

                        case (CampaignStatus.Shipped):
                            throw new UserFriendlyException(L("CampaignShippedCancel"));

                        case (CampaignStatus.ShippingFailed):
                            throw new UserFriendlyException(L("CampaignShipFailCancel"));

                        default:
                            throw new UserFriendlyException(L("CampaignCannotCancel"));

                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }


        }
        #endregion

        public List<string> GetLastLogStatement(int campaignID, int databaseID)
        {
            try
            {
                var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseID);
                if (awsFlag)
                    throw new UserFriendlyException(L("FeatureNotSupported"));

                var sw = new System.Diagnostics.Stopwatch();
                var swNew = new System.Diagnostics.Stopwatch();
                sw.Start();
                var currentStatus = _orderStatusRepository.FirstOrDefault(o => o.OrderID == campaignID && o.iIsCurrent);
                var logList = new List<string>();
                if ((CampaignStatus)currentStatus.iStatus == CampaignStatus.OrderRunning)
                {
                    var dirLoc = _idmsConfigurationCache.GetConfigurationValue("SelectionFilesSQL", databaseID).cValue;
                    var dir = new DirectoryInfo(dirLoc);
                    Logger.Info($"\r\n ---------- Search file on network path initiated for : { campaignID.ToString() } -- current time : { DateTime.Now } ---------- \r\n");
                    swNew.Start();
                    var finfo = dir.GetFileSystemInfos(@"Count_Log_" + campaignID.ToString() + "*.txt");
                    Logger.Info($"\r\n ---------- Search file on network path completed : { swNew.Elapsed.TotalSeconds } -- { campaignID.ToString() } -- current time : { DateTime.Now }---------- \r\n");
                    swNew.Stop();
                    var lastFileInfo = finfo.OrderByDescending(info => info.LastAccessTime).FirstOrDefault();
                    if (lastFileInfo != null)
                    {
                        Logger.Info($"\r\n ---------- File reading and processing initiated for : { campaignID.ToString() } -- current time : { DateTime.Now } ---------- \r\n");
                        swNew.Start();
                        var inStream = new FileStream(lastFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using (var sr = new StreamReader(inStream, Encoding.UTF8))
                        {
                            var line = string.Empty;
                            var sLine = string.Empty;
                            while ((line = sr.ReadLine()) != null)
                            {
                                sLine = ProcessLogLine(line);
                                if (!string.IsNullOrWhiteSpace(sLine))
                                    logList.Add(sLine);
                            }
                        }
                        inStream.Dispose();
                        Logger.Info($"\r\n ---------- File reading and processing completed : { swNew.Elapsed.TotalSeconds } -- { campaignID.ToString() } -- current time : { DateTime.Now }---------- \r\n");
                        swNew.Stop();
                        logList.Reverse();
                    }

                }
                Logger.Info($"\r\n ---------- File reading and processing total time : { sw.Elapsed.TotalSeconds } -- { campaignID.ToString() } -- current time : { DateTime.Now }---------- \r\n");
                sw.Stop();
                return logList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private string ProcessLogLine(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                if (line.StartsWith("#IQCONID#"))
                    return string.Empty;

                line = line.Trim('#');
            }
            return line;
        }
    }
}