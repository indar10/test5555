using System;
using System.Linq;
using Abp.UI;
using Abp.AspNetZeroCore.Net;
using Infogroup.IDMS.CampaignXTabReports.Dtos;
using System.Collections.Generic;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Dto;
using System.IO;
using Infogroup.IDMS.CampaignXTabReports;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.CampaignMultiColumnReports;
using System.Collections;
using Infogroup.IDMS.Helper;
using System.Diagnostics;
using Infogroup.IDMS.Databases;
using Microsoft.AspNetCore.StaticFiles;
using Infogroup.IDMS.Common;

namespace Infogroup.IDMS.Campaigns
{
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        #region Fetch Xtab Reports

        private List<GetCampaignXTabReportsListForView> GetExistingReports(int campaignID, int databaseID)
        {
            var xtabReports = new List<GetCampaignXTabReportsListForView>();
            xtabReports.AddRange(_customCampaignXtabReportsRepository.GetAllCampaignXtabReports(campaignID, databaseID));
            xtabReports.AddRange(Enumerable.Repeat(
            new GetCampaignXTabReportsListForView
            {
                cXField = string.Empty,
                cYField = string.Empty,
                cTypeName = "Net",
                IsXTab = "Campaign",
                cType = "N",
                ID = 0,
                Action = ActionType.None,
                cSegmentNumbers = string.Empty
            }, 4));
            return xtabReports;
        }

        private GetXtabReportsDataDto FetchXtabData(int campaignID, int databaseID)
        {
            var emptyDropDownOutputDto = new DropdownOutputDto { Label = string.Empty, Value = string.Empty };
            var getXTabReports = new GetXtabReportsDataDto
            {
                XFieldDropdown = new List<DropdownOutputDto> { emptyDropDownOutputDto },
                YFieldDropdown = new List<DropdownOutputDto> { emptyDropDownOutputDto }
            };
            try
            {
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var lstLookUptest = _lookupCache.GetLookUpFields("XTABFIELDS", databaseID.ToString());
                Logger.Info($"XTABFIELDS fetch from cache/DB time:{sw.Elapsed.TotalSeconds}");
                sw.Restart();
                var lstLookUp = lstLookUptest.Select(xtab => new DropdownOutputDto { Label = xtab.cDescription, Value = xtab.cField })
                    .ToList();
                Logger.Info($"XTABFIELDS select from cache/DB time:{sw.Elapsed.TotalSeconds}");
                sw.Restart();
                lstLookUp.AddRange(_lookupCache.GetXTabExternalFields(databaseID));
                Logger.Info($"XTABFIELDS fetch externalfields time:{sw.Elapsed.TotalSeconds}");
                if (lstLookUp != null && lstLookUp.Count > 0)
                {
                    lstLookUp = lstLookUp.OrderBy(itm => itm.Label).ToList();
                    var specialSICindex = lstLookUp.FindIndex(ind => ind.Label.Equals("Special SIC"));
                    if (specialSICindex > -1)
                        lstLookUp[specialSICindex].Label = "Selected SIC";
                    getXTabReports.XFieldDropdown.AddRange(lstLookUp);
                    var indexOfZip = lstLookUp.FindIndex(a => a.Label.ToUpper().Equals("ZIP"));
                    var indexOfSelectedSIC = lstLookUp.FindIndex(a => a.Label.Equals("Selected SIC"));
                    if (indexOfZip != -1)
                        lstLookUp.RemoveAt(indexOfZip);
                    if (indexOfSelectedSIC != -1)
                        lstLookUp.RemoveAt(indexOfSelectedSIC);

                    getXTabReports.YFieldDropdown.AddRange(ObjectMapper.Map<List<DropdownOutputDto>>(lstLookUp));
                }
                sw.Restart();
                getXTabReports.XtabRecords = GetExistingReports(campaignID, databaseID);
                sw.Stop();
                Logger.Info($"Get existing reports time:{sw.Elapsed.TotalSeconds}");
                sw1.Stop();
                Logger.Info($"\r\n ----- For campaignId:{campaignID}, Total time for FetchXtabData: {sw1.Elapsed.TotalSeconds} ----- \r\n");
                return getXTabReports;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }


        }

        #endregion

        #region Fetch Multicolumn report
        private GetMultidimensionalReportsDataDto FetchMultiReports(int campaignId, int databaseID)
        {
            var getMultiReports = new GetMultidimensionalReportsDataDto
            {
                fieldsDropdown = new List<DropdownOutputDto>()
            };

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var lstLookUp = GetMultidimensionalFields(campaignId, databaseID);
                if (lstLookUp != null && lstLookUp.Count > 0)
                {
                    lstLookUp = lstLookUp.OrderBy(itm => itm.Label).ToList();
                    ObjectMapper.Map<List<DropdownOutputDto>>(lstLookUp);
                    getMultiReports.fieldsDropdown.AddRange(ObjectMapper.Map<List<DropdownOutputDto>>(lstLookUp));
                }
                getMultiReports.multidimensionalRecords = GetExistingMultidimensionalReports(campaignId);
                sw.Stop();
                Logger.Info($"\r\n ----- For campaignId:{campaignId}, Total time for FetchMultiReports: {sw.Elapsed.TotalSeconds} ----- \r\n");
                return getMultiReports;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<GetCampaignMultidimensionalReportForViewDto> GetExistingMultidimensionalReports(int campaignID)
        {
            var multiReports = new List<GetCampaignMultidimensionalReportForViewDto>();
            var query = _campaignMulticolumnReportBizness.GetAllCampaignMultiDimensionalReports(campaignID);
            multiReports.AddRange(_customCampaignMultiDimensionalReportsRepository.GetAllCampaignMultiDimensionalReports(query.Item1, query.Item2));
            multiReports.AddRange(Enumerable.Repeat(
            new GetCampaignMultidimensionalReportForViewDto
            {
                cFieldDescription = string.Empty,
                cFieldName = string.Empty,
                cTypeName = "Net",
                IsMulti = "Campaign",
                cType = "N",
                ID = 0,
                cSegmentNumbers = string.Empty,
                Action = ActionType.None
            }, 4));
            return multiReports;
        }

        private List<DropdownOutputDto> GetMultidimensionalFields(int campaignId, int databaseID)
        {
            var fields = _idmsConfigurationCache.GetConfigurationValue("SkipFieldsFromMultiColumnReport", databaseID);
            var skipFields = string.Empty;
            var fieldValue = fields.mValue;
            var skippedFieldsList = new List<string>();
            if (!string.IsNullOrEmpty(fieldValue))
            {
                var fieldArray = fieldValue.Split(new Char[] { ',', '|' });
                var builder = new System.Text.StringBuilder();
                foreach (string field in fieldArray)
                {
                    if (!string.IsNullOrEmpty(field.Trim()))
                    {
                        builder.Append($"'{field.Trim()}',");
                        skippedFieldsList.Add(field);
                    }
                }
                skipFields = builder.ToString();
                skipFields = skipFields.TrimEnd(',');
            }
            var dropdownValues = _customCampaignRepository.GetMultiColumnFields(campaignId.ToString(), skipFields).Select(seg => new DropdownOutputDto()
            {
                Value = seg.cFieldName,
                Label = seg.cFieldDescription
            }).ToList();

            var externalBuild = _customCampaignRepository.GetExternalBuildTableIDByOrderID(campaignId);
            if (externalBuild.Count > 0)
            {
                foreach (var extBuildId in externalBuild)
                {
                    var externalFields = _customCampaignRepository.GetExternalDatabaseFields().Where(x => x.cFieldType == "S" && x.ExtBuildId == extBuildId);

                    foreach (var item in externalFields)
                    {
                        if (!skippedFieldsList.Contains(item.cFieldName))
                        {
                            item.cFieldName = $"{item.cTableName.Substring(0, item.cTableName.IndexOf("_"))}.{item.cFieldName}";
                            dropdownValues.Add(new DropdownOutputDto { Value = item.cFieldName, Label = item.cFieldDescription });
                        }
                    }
                }
            }
            return dropdownValues;

        }
        #endregion

        #region Create Edit Delete Reports

        private void CheckXtabReportValidation(List<GetCampaignXTabReportsListForView> xtabReports, int campaignID)
        {
            var validationErrors = new ArrayList();
            var databaseID = _databaseRepository.GetDataSetDatabaseByOrderID(campaignID).Id;
            var segmentLevelXtabFields = _idmsConfigurationCache.GetConfigurationValue("SegmentLevelXTabFields", databaseID).cValue;
            if (!string.IsNullOrEmpty(segmentLevelXtabFields)) segmentLevelXtabFields = segmentLevelXtabFields.ToUpper();

            foreach (var xtabReport in xtabReports)
            {
                if (!string.IsNullOrEmpty(xtabReport.cYDesc) && string.IsNullOrEmpty(xtabReport.cXDesc) && !validationErrors.Contains(L("XtabRquiredValidation")) && xtabReport.Action != ActionType.Delete)
                    validationErrors.Add(L("XtabRquiredValidation"));

                if (xtabReport.cXDesc == xtabReport.cYDesc && !validationErrors.Contains(L("XtabReportCompareMessage")) && xtabReport.Action != ActionType.Delete)
                    validationErrors.Add(L("XtabReportCompareMessage"));

                if (!xtabReport.iXTabBySegment && !string.IsNullOrEmpty(segmentLevelXtabFields) && xtabReport.Action != ActionType.Delete)
                {
                    if ((segmentLevelXtabFields.Split(",").Contains(xtabReport.cXField.ToUpper()) || segmentLevelXtabFields.Split(",").Contains(xtabReport.cYField.ToUpper())) && !validationErrors.Contains(L("SegmentLevelValidation")))
                        validationErrors.Add(L("SegmentLevelValidation"));
                }
            }
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

        private void SaveReports(List<GetCampaignXTabReportsListForView> XTabRecords, int orderID)
        {
            XTabRecords = XTabRecords.Where(xta => !string.IsNullOrEmpty(xta.cYField) || !string.IsNullOrEmpty(xta.cXField) || (xta.Action.Equals(ActionType.Delete) && !xta.ID.Equals(0))).ToList();
            CheckXtabReportValidation(XTabRecords, orderID);
            foreach (var xtabReport in XTabRecords)
            {

                switch (xtabReport.Action)
                {
                    case ActionType.Add:
                        {
                            var CreatecampaignXTabReport = new CampaignXTabReport()
                            {
                                Id = xtabReport.ID,
                                cXField = xtabReport.cXField,
                                cYField = xtabReport.cYField,
                                iXTabBySegment = xtabReport.iXTabBySegment,
                                cType = xtabReport.cType,
                                cSegmentNumbers = xtabReport.cSegmentNumbers,
                                OrderId = orderID,
                                cCreatedBy = _mySession.IDMSUserName,
                                dCreatedDate = DateTime.Now
                            };
                            _customCampaignXtabReportsRepository.InsertAsync(CreatecampaignXTabReport);
                            CurrentUnitOfWork.SaveChanges();
                            break;
                        }
                    case ActionType.Delete:
                        {
                            _customCampaignXtabReportsRepository.DeleteAsync(xtabReport.ID);
                            break;
                        }
                    case ActionType.Edit:
                        {
                            var updatextabReport = _customCampaignXtabReportsRepository.Get(xtabReport.ID);
                            updatextabReport.cXField = xtabReport.cXField;
                            updatextabReport.cYField = xtabReport.cYField;
                            updatextabReport.cType = xtabReport.cType;
                            updatextabReport.cSegmentNumbers = xtabReport.cSegmentNumbers;
                            updatextabReport.iXTabBySegment = xtabReport.iXTabBySegment;
                            updatextabReport.dModifiedDate = DateTime.Now;
                            updatextabReport.cModifiedBy = _mySession.IDMSUserName;
                            _customCampaignXtabReportsRepository.UpdateAsync(updatextabReport);
                            CurrentUnitOfWork.SaveChanges();
                            break;
                        }
                    default:
                        throw new Exception("Unexpected Case");
                }
            }
        }

        private void SaveMultidimensionalReports(List<GetCampaignMultidimensionalReportForViewDto> multidimensionalRecords, int orderID)
        {
            multidimensionalRecords = multidimensionalRecords.Where(x => !string.IsNullOrEmpty(x.cFieldDescription) || (x.Action.Equals(ActionType.Delete) && !x.ID.Equals(0))).ToList();
            foreach (var multidimensionalReport in multidimensionalRecords)
            {

                switch (multidimensionalReport.Action)
                {
                    case ActionType.Add:
                        {
                            var CreatecampaignMultidimensionalReport = new CampaignMultiColumnReport();
                            CreatecampaignMultidimensionalReport = new CampaignMultiColumnReport
                            {
                                Id = multidimensionalReport.ID,
                                cFields = multidimensionalReport.cFieldName,
                                cFieldsDescription = multidimensionalReport.cFieldDescription,
                                iMultiColBySegment = multidimensionalReport.IMultiBySegment,
                                cType = multidimensionalReport.cType,
                                cSegmentNumbers = multidimensionalReport.cSegmentNumbers,
                                OrderId = orderID,
                                cCreatedBy = _mySession.IDMSUserName,
                                dCreatedDate = DateTime.Now
                            };
                            _customCampaignMultiDimensionalReportsRepository.InsertAsync(CreatecampaignMultidimensionalReport);
                            break;
                        }
                    case ActionType.Delete:
                        {
                            _customCampaignMultiDimensionalReportsRepository.DeleteAsync(multidimensionalReport.ID);
                            break;
                        }
                    case ActionType.Edit:
                        {
                            var updateMultidimesionalReport = _customCampaignMultiDimensionalReportsRepository.Get(multidimensionalReport.ID);
                            updateMultidimesionalReport.cFields = multidimensionalReport.cFieldName;
                            updateMultidimesionalReport.cFieldsDescription = multidimensionalReport.cFieldDescription;
                            updateMultidimesionalReport.cType = multidimensionalReport.cType;
                            updateMultidimesionalReport.cSegmentNumbers = multidimensionalReport.cSegmentNumbers;
                            updateMultidimesionalReport.iMultiColBySegment = multidimensionalReport.IMultiBySegment;
                            updateMultidimesionalReport.dModifiedDate = DateTime.Now;
                            updateMultidimesionalReport.cModifiedBy = _mySession.IDMSUserName;

                            _customCampaignMultiDimensionalReportsRepository.UpdateAsync(updateMultidimesionalReport);
                            break;
                        }
                    default:
                        throw new Exception("Unexpected Case");
                }
            }
        }

        #endregion

        #region Download Reports
        public FileDto DownloadMulticolumnReport(int campaignID)
        {
            try
            {
                var filePath = string.Empty;
                var sFileName = string.Empty;
                var destFileName = string.Empty;
                var fileExtension = ".csv";
                var database = _databaseRepository.GetDataSetDatabaseByOrderID(campaignID);
                var isAWSConfigured = _idmsConfigurationCache.IsAWSConfigured(database.Id);
                if (isAWSConfigured)
                {
                    var directoryPath = _idmsConfigurationCache.GetConfigurationValue("ReportsPath", database.Id)?.cValue;
                    destFileName = $"{campaignID}_multidim_reports.zip";
                    string contentType;
                    new FileExtensionContentTypeProvider().TryGetContentType(destFileName, out contentType);
                    return new FileDto($"{directoryPath}{destFileName}", contentType, destFileName, isAWS: true);
                }
                else
                {
                    var getMultiColumnExtension = _idmsConfigurationCache.GetConfigurationValue("MultiColumnReportDelimiter", database.Id);
                    if (getMultiColumnExtension != null && getMultiColumnExtension.cValue != ",")
                    {
                        fileExtension = ".txt";
                    }

                    filePath = _idmsConfigurationCache.GetConfigurationValue("MultiColReportsPath", 0)?.cValue ?? null;
                    sFileName = $"{database.Id}_{campaignID}_*{fileExtension}";
                    destFileName = $"{Path.GetFileNameWithoutExtension(sFileName.Replace("_*", ""))}.zip";
                    ZipUtil.Compress(filePath, $"{filePath}\\Temp\\", sFileName);
                    return new FileDto($"{filePath}\\Temp\\{destFileName}", MimeTypeNames.ApplicationZip, destFileName);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public FileDto DownloadXtabReport(int campaignID)
        {
            try
            {
                var database = _databaseRepository.GetDataSetDatabaseByOrderID(campaignID);
                var filePath = string.Empty;
                var fileName = string.Empty;
                var destFileName = string.Empty;

                var isAWSConfigured = _idmsConfigurationCache.IsAWSConfigured(database.Id);
                if (isAWSConfigured)
                {
                    var directoryPath = _idmsConfigurationCache.GetConfigurationValue("ReportsPath", database.Id)?.cValue;
                    var s3Util = new S3Utilities();
                    var files = s3Util.ListFilesFromUri($"{directoryPath}CountReports_{campaignID}_");
                    var reportFile = files.OrderByDescending(file=>file.LastModified).FirstOrDefault();
                    if (reportFile == null) throw new UserFriendlyException("FileNotFound");
                    fileName = reportFile.Key.Split("/").LastOrDefault();
                    string contentType;
                    new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
                    return new FileDto($"{directoryPath}{fileName}", contentType, fileName, isAWS: true);
                }
                else
                {
                    var fileAttachmentPath = _idmsConfigurationCache.GetConfigurationValue("FileAttachmentPath", 0)?.cValue ?? null;
                    var customRuleDatabaseIDs = _idmsConfigurationCache.GetConfigurationValue("CustomRuleDatabaseIDs", 0)?.cValue ?? null;
                    var totalSegmentCount = _segmentRepository.Query(p => p.Where(q => q.OrderId.Equals(campaignID) && q.iIsOrderLevel.Equals(false))).Count();
                    var combinedXTabSegmentLimit = _idmsConfigurationCache.GetConfigurationValue("CombineXTabSegmentLimit", database.Id).cValue ?? null;
                    if (!string.IsNullOrEmpty(customRuleDatabaseIDs))
                    {
                        var dbIDs = customRuleDatabaseIDs.Split(',').Select(v => int.Parse(v)).ToList();
                        if (dbIDs.Contains(database.Id))
                        {
                            if (!fileAttachmentPath.EndsWith(@"\"))
                                fileAttachmentPath += @"\";
                            var dir = new DirectoryInfo(fileAttachmentPath);
                            fileName = (from file in dir.EnumerateFiles(String.Format("CountReports_{0}_*.xlsx", campaignID), SearchOption.TopDirectoryOnly)
                                        orderby file.LastWriteTimeUtc descending
                                        select file.Name).Distinct().FirstOrDefault();
                        }
                    }

                    if (!string.IsNullOrEmpty(fileName) && totalSegmentCount <= Convert.ToInt32(combinedXTabSegmentLimit))
                    {
                        filePath = $"{fileAttachmentPath}{fileName}";
                        return new FileDto(filePath, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet, fileName);
                    }
                    else
                    {
                        fileName = $"{database.Id}_{campaignID}_*.xlsx";
                        destFileName = $"{Path.GetFileNameWithoutExtension(fileName.Replace("_*", ""))}.zip";
                        filePath = _idmsConfigurationCache.GetConfigurationValue("XTabReportsPath", 0).cValue ?? null;
                        ZipUtil.Compress(filePath, $"{filePath}\\Temp\\", fileName);
                        return new FileDto($"{filePath}\\Temp\\{destFileName}", MimeTypeNames.ApplicationZip, destFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion
    }
}
