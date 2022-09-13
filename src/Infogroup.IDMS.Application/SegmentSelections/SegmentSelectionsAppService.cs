using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Castle.DynamicLinqQueryBuilder;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.Campaigns.Exporting;
using Infogroup.IDMS.Configuration;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.ExternalBuildTableDatabases;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.SegmentPrevOrderses;
using Infogroup.IDMS.Segments;
using Infogroup.IDMS.SegmentSelections.Dtos;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.UserSavedSelectionDetails;
using Infogroup.IDMS.UserSavedSelections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Castle.Core.Logging;
using System.Diagnostics;
using Infogroup.IDMS.Common;

namespace Infogroup.IDMS.SegmentSelections
{
    [AbpAuthorize]
    public partial class SegmentSelectionsAppService : IDMSAppServiceBase, ISegmentSelectionsAppService
    {
        private readonly IRepository<Campaign, int> _campaignRepository;
        private readonly IRedisLookupCache _lookupCache;
        private readonly ICampaignsExcelExporter _campaignsExcelExporter;
        private readonly ICampaignRepository _customCampaignRepository;
        private readonly IBuildTableLayoutRepository _customBuildTableLayoutRepository;
        private readonly IBuildRepository _customBuildRepository;
        private readonly ISegmentSelectionRepository _customSegmentSelectionRepository;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly ISegmentRepository _customSegmentRepository;
        private readonly IRepository<Segment, int> _segmentRepository;
        private readonly IRepository<OrderStatus> _orderStatusRepository;
        private readonly IRepository<SegmentSelection, int> _segmentSelectionRepository;
        private readonly IAppSession _mySession;
        private readonly IOrderStatusManager _orderStatusManager;
        private readonly CampaignBizness _campaignBizness;
        private readonly IRepository<ExternalBuildTableDatabase, int> _ExternalBuildTableDatabaseRepository;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IRepository<BuildTable> _buildTableRepository;
        private readonly IRepository<UserSavedSelection> _userSavedSelectionRepository;
        private readonly IRepository<UserSavedSelectionDetail> _userSavedSelectionDetailsRepository;
        private readonly int csvLineLimit = 500;
        private readonly IShortSearch _shortSearch;
        private static IConfigurationRoot _appConfiguration;
        private readonly ISegmentPreviousOrderRepository _campaignHistoryRepository;
        private readonly IBuildTableLayoutManager _buildTableLayoutManager;

        private static readonly object _syncObject = new object();
        private bool awsFlag = false;
        public SegmentSelectionsAppService(
            IRepository<Campaign, int> campaignRepository,
            IRepository<Segment, int> segmentRepository,
            IBuildTableLayoutRepository customBuildTableLayoutRepository,
            ISegmentRepository customSegmentRepository,
            ISegmentSelectionRepository customSegmentSelectionRepository,
            ICampaignRepository customCampaignRepository,
            IRepository<OrderStatus> orderStatusRepository,
            ICampaignsExcelExporter campaignsExcelExporter,
            IDatabaseRepository databaseRepository,
            IRepository<SegmentSelection, int> segmentSelectionRepository,
            IAppSession mySession,
            IOrderStatusManager orderStatusManager,
            CampaignBizness campaignBizness,
            IRedisLookupCache lookupCache,
            IRepository<ExternalBuildTableDatabase, int> ExternalBuildTableDatabaseRepository,
            IBuildRepository customBuildRepository,
            IRedisIDMSConfigurationCache idmsConfigurationCache,
            IRepository<BuildTable> buildTableRepository,
            IRepository<UserSavedSelection> userSavedSelectionRepository,
            IRepository<UserSavedSelectionDetail> userSavedSelectionDetailsRepository,
            ISegmentPreviousOrderRepository campaignHistoryRepository,
            IBuildTableLayoutManager buildTableLayoutManager,
            IShortSearch shortSearch,
            IHostingEnvironment env
        )
        {
            Logger = NullLogger.Instance;
            _databaseRepository = databaseRepository;
            _campaignRepository = campaignRepository;
            _segmentRepository = segmentRepository;
            _campaignsExcelExporter = campaignsExcelExporter;
            _customSegmentRepository = customSegmentRepository;
            _customCampaignRepository = customCampaignRepository;
            _orderStatusRepository = orderStatusRepository;
            _segmentSelectionRepository = segmentSelectionRepository;
            _customSegmentSelectionRepository = customSegmentSelectionRepository;
            _mySession = mySession;
            _orderStatusManager = orderStatusManager;
            _campaignBizness = campaignBizness;
            _customBuildTableLayoutRepository = customBuildTableLayoutRepository;
            _customBuildRepository = customBuildRepository;
            _ExternalBuildTableDatabaseRepository = ExternalBuildTableDatabaseRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
            _appConfiguration = env.GetAppConfiguration();
            _buildTableRepository = buildTableRepository;
            _userSavedSelectionRepository = userSavedSelectionRepository;
            _userSavedSelectionDetailsRepository = userSavedSelectionDetailsRepository;
            _lookupCache = lookupCache;
            _shortSearch = shortSearch;
            _campaignHistoryRepository = campaignHistoryRepository;
            _buildTableLayoutManager = buildTableLayoutManager;
        }


        private static Expression<Func<SegmentSelection, bool>> FilterBySegmentID(int segmentID)
        {
            return x => x.SegmentId == segmentID;
        }
        public string GetLayoutNameFromCampaignId(int campaignId)
        {
            try
            {
                return _campaignRepository.Get(campaignId).cExportLayout;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task CreateSegmentSelectionDetails(SegmentSelectionSaveDto input, bool isFromSelection = false)
        {
            awsFlag = _idmsConfigurationCache.IsAWSConfigured(input.DatabaseId);
           
            var sw = new Stopwatch();
            var swRule = new Stopwatch();
            //WriteLog($"CampaignId|SegmentId|ValueMode(F/T)|Text|SegmentSelectionId|Field|ElapsedTime(ms)");
            sw.Start();
            var segmentSelectionID = 0;
            var cPath = string.Empty;
            //var cPath = @"D:\Test";
            var nonUpperCaseFields = _idmsConfigurationCache.GetConfigurationValue("NOT_TO_BE_UPPER_CASED", input.DatabaseId).cValue.ToUpper().Split(',').ToList();
            try
            {
                //Delete the entities
                if (input.deletedSelections != null && input.deletedSelections.Count > 0)
                {
                    var swDelete = new Stopwatch();
                    foreach (var value in input.deletedSelections)
                    {
                        swDelete.Restart();
                        await _segmentSelectionRepository.DeleteAsync(value);
                        WriteLog($"{input.campaignId}|||Delete|{value}||{swDelete.ElapsedMilliseconds}");
                        swDelete.Stop();
                    }
                }
                var cSuppressionPath = _idmsConfigurationCache.GetConfigurationValue("LargeSuppressionPath", input.DatabaseId)?.cValue;
               // var cSuppressionPath = @"D:\Dump\";

                foreach (var item in input.selections)
                {
                    if (item.isRuleUpdated > 1)
                        continue;
                    swRule.Restart();
                    var segmentSelection = ObjectMapper.Map<SegmentSelection>(item);
                    var isUpdated = false;
                    if (string.IsNullOrWhiteSpace(segmentSelection.cSystemFileName))
                        isUpdated = true;

                    segmentSelection.cDescriptions = segmentSelection.cDescriptions ?? string.Empty;
                    if (segmentSelection.cQuestionDescription.Equals("Zip Radius") || segmentSelection.cValueMode.Equals("F"))
                    {
                        if (string.IsNullOrEmpty(cPath))
                        {
                            if (awsFlag)
                                cPath = _idmsConfigurationCache.GetConfigurationValue("SelectionFilesSybase", input.DatabaseId).cValue;
                            else
                                cPath = _idmsConfigurationCache.GetConfigurationValue("SelectionFilesSQL", input.DatabaseId).cValue;
                        }
                        if (segmentSelection.cQuestionDescription.Equals("Zip Radius"))
                        {
                            if (item.isDirectFileUpload.Equals(1))
                            {
                                segmentSelection.cFileName = segmentSelection.cValues = $"{cSuppressionPath}{segmentSelection.cValues}";
                            }
                            segmentSelection = ProcessZipRadius(segmentSelection, cPath);
                        }
                        if (segmentSelection.cValueMode.Equals("F") && !string.IsNullOrEmpty(segmentSelection.cFileName))
                        {
                            if (item.isRuleUpdated.Equals(0))
                            {
                                segmentSelection.cCreatedBy = _mySession.IDMSUserName;
                                segmentSelection.dCreatedDate = DateTime.Now;
                                var swFtypeInsert = new Stopwatch();
                                swFtypeInsert.Restart();
                                segmentSelectionID = await _segmentSelectionRepository.InsertAndGetIdAsync(segmentSelection);
                                if (isFromSelection)
                                    await CurrentUnitOfWork.SaveChangesAsync();
                                WriteLog($"{input.campaignId}|{item.SegmentId}|F|Insert||{item.cQuestionFieldName}|{swFtypeInsert.ElapsedMilliseconds}");
                                swFtypeInsert.Stop();
                            }
                            var srcFile = Path.Combine(cPath, segmentSelection.cFileName);
                            var ext = Path.GetExtension(segmentSelection.cFileName);
                            if (String.IsNullOrWhiteSpace(segmentSelection.cSystemFileName))
                            {
                                segmentSelection.cSystemFileName = item.isRuleUpdated.Equals(0) ? $"FileUpload_{input.campaignId}_{item.SegmentId}_{segmentSelectionID}_1{ext}" : $"FileUpload_{input.campaignId}_{item.SegmentId}_{item.Id}_1{ext}";
                            }

                            var desFile = Path.Combine(cPath, segmentSelection.cSystemFileName);
                            
                            if (segmentSelection.cQuestionDescription != "Zip Radius" && segmentSelection.cFileName.Contains("_") && !segmentSelection.cSystemFileName.Contains(cSuppressionPath) && segmentSelectionID != 0)
                            {
                                if (segmentSelection.cFileName.Length > 19 && char.IsDigit(segmentSelection.cFileName, 17))
                                {
                                    segmentSelection.cFileName = segmentSelection.cFileName.Substring(19);
                                }
                            }
                            else if(segmentSelection.cQuestionDescription != "Zip Radius" && segmentSelectionID == 0  && !segmentSelection.cSystemFileName.Contains(cSuppressionPath))
                            {
                                var result = char.IsDigit(segmentSelection.cFileName, 0);
                               
                                if (result)
                                {
                                    var result1 = char.IsDigit(segmentSelection.cFileName, 17);
                                    if (result1)
                                    {
                                        segmentSelection.cFileName = segmentSelection.cFileName.Substring(19);
                                    }
                                }
                                else
                                {
                                    segmentSelection.cFileName = segmentSelection.cFileName;
                                }
                            }
                            var swMove = new Stopwatch();
                            swMove.Restart();
                            if ((segmentSelection.cQuestionDescription.Equals("Zip Radius") || item.isDirectFileUpload.Equals(0)) && isUpdated)
                            {
                                if (awsFlag)
                                {
                                    var client = new S3Utilities();
                                    var bucketName = cPath.Substring(cPath.IndexOf("//") == -1 ? cPath.IndexOf(cPath) : cPath.IndexOf("//") + 2).TrimEnd('/');
                                    client.CopyObject(bucketName, Path.GetFileName(srcFile), bucketName, segmentSelection.cSystemFileName).Wait();
                                }
                                else
                                    File.Copy(srcFile, desFile, true);
                            }                            
                            WriteLog($"{input.campaignId}|{item.SegmentId}|F|Move|{item.Id}||{swMove.ElapsedMilliseconds}");
                            swMove.Stop();
                            if (item.isRuleUpdated.Equals(0))
                            {
                                var swFtypeUpdate = new Stopwatch();
                                swFtypeUpdate.Restart();
                                await _segmentSelectionRepository.UpdateAsync(segmentSelection);
                                WriteLog($"{input.campaignId}|{item.SegmentId}|F|Update|{segmentSelection.Id}|{item.cQuestionFieldName}|{swFtypeUpdate.ElapsedMilliseconds}");
                                swFtypeUpdate.Stop();
                            }
                        }
                    }

                    if (segmentSelection.cValueMode.Equals("T") && !string.IsNullOrWhiteSpace(item.cValues))
                    {
                        var updatedcValue = item.cValues;
                        if (!nonUpperCaseFields.Contains(item.cQuestionFieldName.ToUpper()))
                        {
                            updatedcValue = item.cValues.ToUpper();
                            segmentSelection.cValues = updatedcValue;
                        }
                        segmentSelection.cValues = ManipluateText(updatedcValue, input.DatabaseId);
                    }

                    if (item.isRuleUpdated.Equals(0) && segmentSelection.cValueMode != "F")
                    {
                        ReplaceSingleQuotes(segmentSelection);
                        segmentSelection.cCreatedBy = _mySession.IDMSUserName;
                        segmentSelection.dCreatedDate = DateTime.Now;
                        var swTtypeInsert = new Stopwatch();
                        swTtypeInsert.Restart();
                        await _segmentSelectionRepository.InsertAsync(segmentSelection);
                        if (isFromSelection)
                            await CurrentUnitOfWork.SaveChangesAsync();
                        WriteLog($"{input.campaignId}|{item.SegmentId}|T|Insert||{item.cQuestionFieldName}|{swTtypeInsert.ElapsedMilliseconds}");
                        swTtypeInsert.Stop();
                    }
                    else if (item.isRuleUpdated.Equals(1))
                    {
                        ReplaceSingleQuotes(segmentSelection);
                        segmentSelection.cModifiedBy = _mySession.IDMSUserName;
                        segmentSelection.dModifiedDate = DateTime.Now;
                        var swTtypeUpdate = new Stopwatch();
                        swTtypeUpdate.Restart();
                        await _segmentSelectionRepository.UpdateAsync(segmentSelection);
                        WriteLog($"{input.campaignId}|{item.SegmentId}|T|Update|{segmentSelection.Id}|{item.cQuestionFieldName}|{swTtypeUpdate.ElapsedMilliseconds}");
                        swTtypeUpdate.Stop();
                    }
                    swRule.Stop();
                }
                if (!isFromSelection)
                    await CurrentUnitOfWork.SaveChangesAsync();

                await _orderStatusManager.UpdateOrderStatus(input.campaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                sw.Stop();
            }
            catch (Exception e)
            {
                var exceptionMessage = string.Empty;
                var innerExceptionMessage = string.Empty;
                exceptionMessage = !string.IsNullOrEmpty(e.Message) ? e.Message : string.Empty;
                if (e.InnerException != null)
                {
                    innerExceptionMessage = !string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : string.Empty;
                }
                throw new UserFriendlyException(exceptionMessage + innerExceptionMessage);
            }
        }

        private string ManipluateText(string input, int databaseId)
        {
            var charArray = input.Trim().Split('\n');
            var convertToCSVExceedLimit = _idmsConfigurationCache.GetConfigurationValue("ConvertToCSVExceedLimit", databaseId).cValue;
            var exceedLimit = string.IsNullOrEmpty(convertToCSVExceedLimit) ? csvLineLimit : Convert.ToInt32(convertToCSVExceedLimit);
            if (charArray.Count() >= exceedLimit)
            {
                throw new UserFriendlyException(L("NumberOfLineLimit", exceedLimit));
            }
            if (charArray.Count() > 1)
            {
                var builder = new StringBuilder();
                foreach (var item in charArray)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var enteredText = item.Trim().Trim(',');
                        builder.Append($"{enteredText.Trim()},");
                    }
                }
                input = builder.ToString().TrimEnd(',');
            }
            return input;
        }
        private void ReplaceSingleQuotes(SegmentSelection selection)
        {
            const string Mask = "^_^";
            const string DoubleQuotes = "''";
            const string SingleQuotes = "'";
            selection.cValues = selection.cValues.Replace(DoubleQuotes, Mask)
                .Replace(SingleQuotes, DoubleQuotes)
                .Replace(Mask, DoubleQuotes);
        }

        public int GetSegmentIdForOrderLevel(int campaignId)
        {
            var segmentId = 0;
            var segment = (from seg in _segmentRepository.GetAll()
                           join segmentSelection in _segmentSelectionRepository.GetAll()
                           on seg.Id equals segmentSelection.SegmentId
                           where seg.OrderId == campaignId
                           orderby seg.iDedupeOrderSpecified
                           select new { seg.Id, seg.iDedupeOrderSpecified }).Distinct().FirstOrDefault();

            if (segment == null)
            {
                segmentId = GetSegmentIdASC(campaignId);
            }
            else
            {
                if (segment.iDedupeOrderSpecified != 0)
                {
                    var segmentIdFromSegmentTable = GetSegmentIdASC(campaignId);
                    segmentId = segmentIdFromSegmentTable != segment.Id ? segmentIdFromSegmentTable : segment.Id;
                }
                else
                    segmentId = segment.Id;
            }
            return segmentId;
        }

        private int GetSegmentIdASC(int campaignId)
        {
            return _segmentRepository.GetAll()
                .Where(w => w.OrderId == campaignId && w.iDedupeOrderSpecified != 0)
                .OrderBy(o => o.iDedupeOrderSpecified)
                .Select(o => o.Id).FirstOrDefault();
        }

        private SegmentSelection ProcessZipRadius(SegmentSelection segmentSelection, string cPath)
        {
            if (segmentSelection.cValueMode == "T")
            {
                var sCommaSeparateZips = new StringBuilder(segmentSelection.cValues);
                if (segmentSelection.cValues.EndsWith("\n") && segmentSelection.cValues.Length > 2)
                {
                    sCommaSeparateZips = sCommaSeparateZips.Remove(segmentSelection.cValues.Length - 1, 1);
                }

                //check when there is radius key found. - Zipradius group
                if (sCommaSeparateZips.ToString().ToLowerInvariant().Contains("radius"))
                {
                    var result = Regex.Split(sCommaSeparateZips.ToString(), "\r\n|\r|\n");
                    var sbZipRadius = new StringBuilder();
                    decimal radiusVal = 0;
                    foreach (string item in result)
                    {
                        string line = item;

                        GetUpdatedZipRadius(sbZipRadius, ref line, ref radiusVal);
                    }
                    sCommaSeparateZips = sbZipRadius;
                }

                sCommaSeparateZips = sCommaSeparateZips.Replace("\n", ",");
                var stopWatchForSaveFileZipRadiusAndGetZipSelection = new Stopwatch();
                stopWatchForSaveFileZipRadiusAndGetZipSelection.Start();

                segmentSelection.cFileName = SaveFileZipRadius(cPath, _customSegmentSelectionRepository.GetZipSelection(sCommaSeparateZips.ToString()));
                WriteLog($"|{segmentSelection.SegmentId}|T|Save zip radius file|||{stopWatchForSaveFileZipRadiusAndGetZipSelection.ElapsedMilliseconds}");
                stopWatchForSaveFileZipRadiusAndGetZipSelection.Stop();
                segmentSelection.cValueMode = "F";
            }
            else if (segmentSelection.cValueMode == "F")
            {
                var sOldfilename = string.Empty;
                var fileTempPath = string.Empty; //Store full file path

                //if file is uploded through path               
                if (HttpUtility.UrlDecode(segmentSelection.cFileName).Contains("\\") || HttpUtility.UrlDecode(segmentSelection.cFileName).Contains("s3"))
                {
                    //Sets the entire path as Value
                    sOldfilename = HttpUtility.UrlDecode(segmentSelection.cFileName);
                    segmentSelection.cValues = sOldfilename;
                    fileTempPath = HttpUtility.UrlDecode(segmentSelection.cFileName); //Get full file path
                }
                else
                {
                    fileTempPath = Path.Combine(cPath, segmentSelection.cFileName);
                }

                if (fileTempPath != string.Empty)
                {
                    //Create a new file with Zip 
                    var stopWatchForSaveFileZipRadius = new Stopwatch();
                    stopWatchForSaveFileZipRadius.Start();
                    segmentSelection.cFileName = SaveFileZipRadius(cPath, fileTempPath);
                    WriteLog($"|{segmentSelection.SegmentId}|F|Save zip radius file|||{stopWatchForSaveFileZipRadius.ElapsedMilliseconds}");
                    stopWatchForSaveFileZipRadius.Stop();
                    segmentSelection.cValueMode = "F";
                }
            }
            return segmentSelection;
        }

        private void GetUpdatedZipRadius(StringBuilder sbZipRadius, ref string line, ref decimal radiusVal)
        {
            //if ',' exists in the line,it will be replace by '-'
            line = line.Replace(',', '-');

            if (Regex.IsMatch(line, @"^(\w{6})") && radiusVal != 0 || line.ToLowerInvariant().Contains("radius"))
            {
                if (line.ToLowerInvariant().Contains("radius"))
                {
                    string[] arrRadius = line.Split('=');
                    if (arrRadius.Length > 1)
                        decimal.TryParse(arrRadius[1].ToString(), out radiusVal);
                    else
                        radiusVal = 0;
                }
            }
            else
            {
                radiusVal = 0;
            }

            //Check for Zip Radius Format
            if (Regex.IsMatch(line, @"^(\w{6})") && radiusVal != 0)
            {
                sbZipRadius.Append(string.Format("{0}-{1},", line, radiusVal));
            }
        }

        private string SaveFileZipRadius(string sPath, StringBuilder sZipCodes)
        {
            string sFileName;
            sFileName = $"{Guid.NewGuid()}.txt";
            // Add AWS logic
            if (awsFlag)
            {
                var sZipCodesArray = Encoding.UTF8.GetBytes(sZipCodes.ToString());
                var ms = new MemoryStream(sZipCodesArray);
                var bucketName = sPath.Substring(sPath.IndexOf("//") == -1 ? sPath.IndexOf(sPath) : sPath.IndexOf("//") + 2).TrimEnd('/');
                var uploadToS3 = new S3Utilities();
                uploadToS3.FileUploadToS3(ms, bucketName, sFileName);
            }
            else
            {
                using (StreamWriter outfile =
                  new StreamWriter(sPath + @"\\" + sFileName))
                {
                    outfile.Write(sZipCodes.ToString());
                }
            }
            return sFileName;
        }

        private string SaveFileZipRadius(string sPath, string fileTempPath)
        {
            string sFileName;
            sFileName = $"{ Guid.NewGuid().ToString().Replace("|", "")}.txt";
            // AWS logic
            if(awsFlag)
            {
                var client = new S3Utilities();
                var srcS3Path = fileTempPath.Substring(0, fileTempPath.Length - Path.GetFileName(fileTempPath).Length);
                var srcBucketname = srcS3Path.Substring(srcS3Path.IndexOf("//") == -1 ? srcS3Path.IndexOf(srcS3Path) : srcS3Path.IndexOf("//") + 2).TrimEnd('/');
                var destBucketname = sPath.Substring(sPath.IndexOf("//") == -1 ? sPath.IndexOf(sPath) : sPath.IndexOf("//") + 2).TrimEnd('/');
                client.CopyObject(srcBucketname, Path.GetFileName(fileTempPath), destBucketname, sFileName).Wait();               
            }
            else
            {
                string destFile = Path.Combine(sPath.Replace("|", ""), sFileName);
                destFile = destFile.Replace("|", "");
                File.Copy(fileTempPath, destFile);
            }
            return sFileName;
        }

        public async Task DeleteAll(int segmentId, int campaignId)
        {
            try
            {
                _segmentSelectionRepository.Delete(FilterBySegmentID(segmentId));
                await _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }

        #region GetSelectionFieldsFromDB  
        /// <summary>
        /// Generating JSON from tblsegmentSelection
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="id"></param>//SegmentID
        /// <param name="isSegment"></param>
        /// <param name="isFavorite"></param>
        /// <returns></returns>
        public GetQueryBuilderDetails GetSelectionFieldsNew(int id, string isSegment, int databaseId, int buildId, int mailerId)
        {

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var cRootOperator = "AND";
                var cGroupOp = string.Empty;
                var finalObj = new RootSegmentSelectionDetails();
                var list = new List<GroupSegmentSelectionDetails>();
                List<ColumnDefinition> unmappedfieldDefinition;
                List<ColumnDefinition> mappedfieldDefinition;

                var iBuildLoLID = isSegment == Constants.SegmentLevel ? ((_customSegmentRepository.GetSegmentListCount(id) == 1) ? _customBuildRepository.GetBuildLolID(id, buildId) : 0)
                  : ((_customSegmentSelectionRepository.GetSubSelListCount(id) == 1) ? _customBuildRepository.GetBuildLolID(id, buildId) : 0);

                var result = _customSegmentSelectionRepository.GetSegmentSelectionBySegmentID(id, iBuildLoLID).GroupBy(i => i.iGroupNumber);
                var listRootOperator = (from val in result where val.Key == 2 || val.Key == 1 select val).ToList();
                if (listRootOperator.Any())
                {
                    cRootOperator = listRootOperator.Count() > 1 ? listRootOperator[1].ElementAt(0).cJoinOperator.Trim() : listRootOperator[0].ElementAt(0).cJoinOperator.Trim();
                }

                foreach (var item in result)
                {
                    var bindObj = new GroupSegmentSelectionDetails();
                    var segmentSelection = ObjectMapper.Map<List<BindSegmentSelectionDetails>>(item);
                    cGroupOp = item.FirstOrDefault().cJoinOperator.Trim();

                    bindObj.condition = item.Count() > 1 ? item.ElementAt(1).cJoinOperator.Trim() : cGroupOp;
                    bindObj.rules = segmentSelection;
                    list.Add(bindObj);

                }
                finalObj.condition = cRootOperator;
                finalObj.rules = list;
                var jsonSerializerSegmentSelection = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var segmentSelctionjson = JsonConvert.SerializeObject(finalObj, jsonSerializerSegmentSelection);

                var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

                var allColumnsDefinition = GetDefaultColumnDefinitionsForType(isSegment, iBuildLoLID, databaseId, buildId, mailerId);
                if (iBuildLoLID != 0)
                {
                    unmappedfieldDefinition = allColumnsDefinition.Where(field => field.Data.cFieldDescription.StartsWith("RAW")).ToList();
                    mappedfieldDefinition = allColumnsDefinition.Where(field => !field.Data.cFieldDescription.StartsWith("RAW")).ToList();
                }
                else
                {
                    mappedfieldDefinition = allColumnsDefinition;
                    unmappedfieldDefinition = new List<ColumnDefinition>();
                }
                var fields = JsonConvert.SerializeObject(mappedfieldDefinition, jsonSerializerSettings);
                var unmappedfields = JsonConvert.SerializeObject(unmappedfieldDefinition, jsonSerializerSettings);
                WriteLog($"|{id}||Get Segment Selections|||{sw.ElapsedMilliseconds}");
                sw.Stop();
                return new GetQueryBuilderDetails { FilterDetails = fields, FilterQuery = segmentSelctionjson, UnMappedFilters = unmappedfields, BuildLolId = iBuildLoLID };
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<ColumnDefinition> GetSubSelectSelections(int id, string isSegment, int databaseId, int buildId, int mailerId)
        {
            try
            {
                var iBuildLoLID = isSegment == Constants.SegmentLevel ? ((_customSegmentRepository.GetSegmentListCount(id) == 1) ? _customBuildRepository.GetBuildLolID(id, buildId) : 0)
                 : ((id > 0 && _customSegmentSelectionRepository.GetSubSelListCount(id) == 1) ? _customBuildRepository.GetSubSelBuildLolID(id, buildId) : 0);

                var allColumnsDefinition = GetDefaultColumnDefinitionsForType(isSegment, iBuildLoLID, databaseId, buildId, mailerId);
                return allColumnsDefinition;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        private List<FieldData> GetMainTableFields(int buildId, int mailerId, int iBuildLoLID, string isSegment)
        {

            var fields = _customBuildTableLayoutRepository.GetFieldsForBuildLayout(buildId, mailerId, iBuildLoLID, isSegment).ToList();
            if (isSegment == Constants.SubSelectLevel)
            {
                fields = fields.Where(x => x.cFieldName.ToUpper() != "ZIPRADIUS" && x.cFieldName.ToUpper() != "GEORADIUS").ToList();
            }
            return fields;
        }

        private List<ColumnDefinition> GetDefaultColumnDefinitionsForType(string isSegment, int iBuildLoLID, int databaseId, int buildId, int mailerId)
        {
            var favouriteFilters = new List<string>();
            var lstExternalDB = new List<FieldData>();

            var lstFieldData = GetMainTableFields(buildId, mailerId, iBuildLoLID, isSegment);

            if (isSegment == Constants.SubSelectLevel)
            {
                lstExternalDB = _customBuildRepository.GetExternalTablesByOrderIdForSubSelect(databaseId, mailerId, iBuildLoLID, isSegment);
                if (lstExternalDB != null && lstExternalDB.Count > 0)
                    lstFieldData.AddRange(lstExternalDB);
            }
            else
            {

                var lstExternalDBFields = _customBuildRepository.GetExternalDatabaseFields(databaseId);
                if (lstExternalDBFields != null)
                    lstFieldData.AddRange(lstExternalDBFields);

                favouriteFilters = _customBuildTableLayoutRepository.GetFavouriteFields(databaseId, _mySession.IDMSUserId);

                var favouriteItems = new List<FieldData>();
                foreach (var value in favouriteFilters)
                {
                    var itemToRemove = lstFieldData.SingleOrDefault(r => $"{r.cFieldName}{r.cTableName.Split("_")[0]}".Equals(value));
                    if (itemToRemove != null)
                    {
                        itemToRemove.IsFavourite = true;
                        favouriteItems.Add(itemToRemove);
                        lstFieldData.Remove(itemToRemove);
                    }
                }
                lstFieldData.InsertRange(0, favouriteItems);
            }

            var itemBankColumnDefinitions = new List<ColumnDefinition>();
            foreach (var item in lstFieldData)
            {
                var name = item.cFieldName;

                var title = item.cFieldDescription;

                var type = string.Empty;

                if (item.cDataType == "char" || item.cDataType == "varchar")
                {
                    type = "string";
                }
                else if (item.cDataType == "int" || item.cDataType == "bigint")
                {
                    type = "integer";
                }

                switch (type)
                {
                    case "integer":
                        itemBankColumnDefinitions.Add(new ColumnDefinition
                        {
                            Label = title,
                            Field = name,
                            Value = name,
                            Id = item.ID.ToString(),
                            Type = "integer",
                            Data = new BuildTableLayoutDetails
                            {
                                ID = item.ID,
                                cFieldType = item.cFieldType,
                                cTableName = item.cTableName,
                                iDataLength = item.iDataLength,
                                iShowListBox = item.iShowListBox,
                                iShowTextBox = item.iShowTextBox,
                                iShowDefault = item.iShowDefault,
                                iFileOperations = item.iFileOperations,
                                cFieldDescription = title,
                                cFieldName = name,
                                iBTID = item.iBTID,
                                iIsListSpecific = item.iIsListSpecific
                            },

                            Input = item.iShowListBox ? "select" : "textarea",
                            optgroup = favouriteFilters.Count > 0 ? (item.IsFavourite ? "favourite" : "allFilters") : string.Empty
                        });
                        break;
                    case "string":
                    default:
                        itemBankColumnDefinitions.Add(new ColumnDefinition
                        {
                            Label = title,
                            Field = name,
                            Value = name,
                            Id = item.ID.ToString(),
                            Type = "string",
                            Data = new BuildTableLayoutDetails
                            {
                                ID = item.ID,
                                cFieldType = item.cFieldType,
                                cTableName = item.cTableName,
                                iDataLength = item.iDataLength,
                                iShowListBox = item.iShowListBox,
                                iShowTextBox = item.iShowTextBox,
                                iShowDefault = item.iShowDefault,
                                iFileOperations = item.iFileOperations,
                                cFieldDescription = title,
                                cFieldName = name,
                                iBTID = item.iBTID,
                                iIsListSpecific = item.iIsListSpecific
                            },
                            Input = item.iShowListBox ? "select" : "textarea",
                            Multiple = item.iShowListBox ? true : false,
                            optgroup = favouriteFilters.Count > 0 ? (item.IsFavourite ? "favourite" : "allFilters") : string.Empty

                        });
                        break;
                }
            }
            return itemBankColumnDefinitions;
        }
        #endregion

        #region GetFieldValuesFromDB       
        public List<ValueList> GetFieldValues(string fieldId, int iBuildLoLID)
        {
            try
            {
                return _customBuildRepository.GetValues(fieldId, iBuildLoLID, string.Empty, " desc");
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        public List<DropdownOutputDto> GetSubSelectFieldValues(string fieldId, int iBuildLoLID)
        {
            try
            {
                var fieldValues = _customBuildRepository.GetValues(fieldId, iBuildLoLID, string.Empty, " desc");
                return fieldValues.Select(l => new DropdownOutputDto
                {
                    Value = l.cValue,
                    Label = l.cDescription
                }).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public List<DropdownOutputDto> GetOperators()
        {
            try
            {
                var allOperatorsList = _lookupCache.GetLookUpFields("OPERATION")
                                   .Select(lookup => new DropdownOutputDto
                                   {
                                       Value = lookup.cCode,
                                       Label = lookup.cDescription
                                   }).ToList();
                return allOperatorsList;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        #endregion

        #region Add User Level Default      
        public async Task AddDefaultSelections(int segmentId, int buildId, int databaseId, string channelType)
        {
            SegmentSelection selection;
            try
            {
                var defaultSelections = GetDefautSelections(channelType, databaseId);
                var cMainTableName = GetMainTableNameByBuildID(buildId);
                if (!string.IsNullOrEmpty(cMainTableName))
                {
                    foreach (UserSavedSelectionDetail defaultSelection in defaultSelections)
                    {
                        selection = new SegmentSelection
                        {
                            cQuestionFieldName = defaultSelection.cQuestionFieldName,
                            cQuestionDescription = defaultSelection.cQuestionDescription,
                            cValueMode = defaultSelection.cValueMode,
                            cJoinOperator = defaultSelection.cJoinOperator,
                            cValues = defaultSelection.cValues,
                            cDescriptions = defaultSelection.cDescriptions,
                            cValueOperator = defaultSelection.cValueOperator,
                            cGrouping = defaultSelection.cGrouping,
                            iGroupOrder = defaultSelection.iGroupOrder,
                            SegmentId = segmentId,
                            iGroupNumber = defaultSelection.iGroupNumber,
                            cCreatedBy = _mySession.IDMSUserName,
                            dCreatedDate = DateTime.Now,
                            cFileName = string.Empty,
                            cSystemFileName = string.Empty
                        };
                        if (string.IsNullOrEmpty(defaultSelection.cTableName) || defaultSelection.cTableName.Contains("Main"))
                        {
                            selection.cTableName = cMainTableName;
                        }
                        else if (defaultSelection.cTableName.Contains("Child"))
                        {
                            var sChildTable = defaultSelection.cTableName.Split('_');
                            var mainTableName = cMainTableName.Split('_');
                            selection.cTableName = sChildTable[0] + "_" + mainTableName[1] + "_" + mainTableName[2];
                        }
                        else
                        {
                            selection.cTableName = defaultSelection.cTableName;
                        }
                        await _segmentSelectionRepository.InsertAsync(selection);
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private List<UserSavedSelectionDetail> GetDefautSelections(string channelType, int iDatabaseId)
        {
            var query = (from userSavedSelectionDetail in _userSavedSelectionDetailsRepository.GetAll()
                         join userSavedSelections in _userSavedSelectionRepository.GetAll()
                         on userSavedSelectionDetail.UserSavedSelectionId equals userSavedSelections.Id
                         where userSavedSelections.cChannelType.Equals(channelType)
                         && userSavedSelections.UserID.Equals(_mySession.IDMSUserId)
                         && userSavedSelections.DatabaseId.Equals(iDatabaseId)
                         && userSavedSelections.iIsDefault
                         && userSavedSelections.iIsActive
                         && userSavedSelectionDetail.iIsActive
                         select userSavedSelectionDetail
                              );
            var selections = query.OrderBy(sel => sel.iGroupNumber).ThenBy(sel => sel.Id).ToList();
            return selections;
        }
        public string GetMainTableNameByBuildID(int buildId)
        {
            var buildtable = (from buildTable in _buildTableRepository.GetAll()
                              where buildTable.LK_TableType.Equals("M")
                              && buildTable.BuildId == buildId
                              select buildTable.cTableName).FirstOrDefault();
            return buildtable;
        }
        #endregion

        #region Multi-Field Search 

        public int GetNewMaxGroupId(int segmentId)
        {
            try
            {
                return (_customSegmentSelectionRepository.GetAll().Where(sel => sel.SegmentId == segmentId && sel.iGroupNumber != 999)
                         .Max(sel => (int?)sel.iGroupNumber) ?? 0) + 1;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        private void GroupSelections(int segmentId, int maxGroupId, List<int> selectionList)
        {
            try
            {
                var count = _customSegmentSelectionRepository.GetAll().Count(p => p.cGrouping == "Y" && (p.iGroupNumber != maxGroupId) && selectionList.Contains(p.Id));
                var bShouldTheseGroup = count > 0 ? false : true;

                if (!selectionList.Contains(',') && bShouldTheseGroup)
                {
                    var segmentSelection = _customSegmentSelectionRepository.Get(selectionList[0]);
                    if (maxGroupId == segmentSelection.iGroupNumber)
                    {
                        bShouldTheseGroup = false;
                    }
                }

                if (bShouldTheseGroup)
                {
                    var selections = string.Join(",", selectionList.Select(n => n.ToString()).ToArray());
                    _customSegmentSelectionRepository.GroupSelection(segmentId, maxGroupId, selections);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        #endregion

        public string GetAdminEmailAddress(int databseId)
        {
            try
            {
                var adminEmailAddress = _databaseRepository.GetAll().FirstOrDefault(x => x.Id == databseId).cAdministratorEmail;
                return adminEmailAddress;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public static void WriteLog(string strLog)
        {
            if (_appConfiguration["LogFile:IsLoggingEnabled"] == "True")
            {
                var strFilename = $"CampaignUI-Log {DateTime.Today.ToString("MM-dd-yyyy")}.txt";
                var logFilePath = _appConfiguration["LogFile:Path"];
                var fullPath = Path.Combine(logFilePath, strFilename);

                FileInfo logFileInfo = new FileInfo(fullPath);
                DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                try
                {
                    lock (_syncObject)
                    {
                        using (FileStream fileStream = new FileStream(fullPath, FileMode.Append))
                        {
                            using (StreamWriter log = new StreamWriter(fileStream))
                            {
                                if (fileStream.Length == 0)
                                    log.WriteLine($"DateTime|CampaignId|SegmentId|ValueMode(F/T)|Notes|SegmentSelectionId|cQuestionFieldName|ElapsedTime(ms)");
                                log.WriteLine($"{DateTime.Now}|{strLog}");
                            }
                        }
                    }
                }
                catch { }
            }
        }
    }
    public static class Constants
    {
        public const string OrderLevel = "0";
        public const string SegmentLevel = "1";
        public const string SubSelectLevel = "2";
    }
}
