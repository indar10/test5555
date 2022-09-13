using Abp.AspNetZeroCore.Net;
using Abp.UI;
using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Segments.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using Newtonsoft.Json;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Segments
{
    public partial class SegmentsAppService : IDMSAppServiceBase, ISegmentsAppService
    {
        #region DataPreview
        private List<BuildTableLayoutDto> GetBuildTableLayoutForSegmentDump(int buildId, string[] columnNames)
        {

            var listOfbuildTablelayout = (from btl in _buildTableLayoutRepository.GetAll()
                                          join bt in _buildTableRepository.GetAll()
                                          on btl.BuildTableId equals bt.Id
                                          where (!btl.cFieldName.StartsWith("_Deciles") &&
                                                !btl.cFieldName.StartsWith("_Score") &&
                                                btl.cFieldName != "zipradius" &&
                                                btl.cFieldName != "Georadius" && btl.cFieldName != "DropFlag") &&
                                                bt.BuildId == buildId && columnNames.Contains(btl.cFieldName.ToUpper())
                                          select new BuildTableLayoutDto { cFieldName = btl.cFieldName, cFieldDescription = btl.cFieldDescription }).ToList();
            return listOfbuildTablelayout;

        }
        private List<BuildTableLayoutDto> GetExternalBuildTableLayoutEntities(int campaignId)
        {

            var listOfbuildTablelayout = (from c in _campaignRepository.GetAll()
                                          join b in _buildRepository.GetAll() on c.BuildID equals b.Id
                                          join e in _externalBuildTableDatabaseRepository.GetAll() on b.DatabaseId equals e.DatabaseID
                                          join bt in _buildTableRepository.GetAll() on e.BuildTableID equals bt.Id
                                          join bl in _buildTableLayoutRepository.GetAll() on bt.Id equals bl.BuildTableId
                                          where c.Id.Equals(campaignId)
                                          select new BuildTableLayoutDto { cFieldName = bl.cFieldName, cFieldDescription = bl.cFieldDescription }).ToList();
            return listOfbuildTablelayout;

        }

        #region Fetch
        public async Task<SegmentDataPreviewDto> GetRecordDumpAsync(int segmentId, bool isExportLayout)
        {
            try
            {
                var layoutExtObject = new List<BuildTableLayoutDto>();
                var endpointAddress = _appConfiguration[@"Services:Uri"];
                var service = new IDMSCommonService.IDMSIQServiceClient(endpointAddress);
                TimeSpan timeSpan = new TimeSpan(0, 5, 0);
                service.Endpoint.Binding.SendTimeout = timeSpan;
                var response = await service.ListOfSegmentDumpAsync(segmentId, 25, isExportLayout);
                var segmentData = _customSegmentRepository.Get(segmentId);
                var campaignObject = _campaignRepository.Get(segmentData.OrderId);
                var campaignId = campaignObject.Id;
                var buildId = campaignObject.BuildID;
                var segmentDataPreviewData = new SegmentDataPreviewDto
                {
                    Description = $"{L("SegmentDescLabel")}{(segmentData.cDescription.Length > 45 ? segmentData.cDescription.Substring(0, 45) + "..." : segmentData.cDescription)} ({segmentId})",
                    TooltipDescription = segmentData.cDescription,
                    isExportLayoutCheckBoxVisible = _campaignExportLayoutRepository.GetAll().Any(t => t.OrderId == campaignId) ? true : false
                };
                if (!string.IsNullOrEmpty(response.ListOfSegmentDumpResult))
                {
                    var dumpDataTable = JsonConvert.DeserializeObject<DataTable>(response.ListOfSegmentDumpResult);
                    var columnNames = (from dc in dumpDataTable.Columns.Cast<DataColumn>()
                                       select $"{dc.ColumnName.ToUpper()}").ToArray();
                    var buildTableLayoutResult = GetBuildTableLayoutForSegmentDump(buildId, columnNames);
                    if (isExportLayout)
                        layoutExtObject = GetExternalBuildTableLayoutEntities(campaignId);
                    segmentDataPreviewData.Columns = new List<DropdownOutputDto>();
                    foreach (DataColumn col in dumpDataTable.Columns)
                    {
                        var dropdownOutputDto = new DropdownOutputDto();
                        if (col.ColumnName == "ckeycode1")
                        {
                            foreach (DataRow dr in dumpDataTable.Rows)
                                dr.SetField("ckeycode1", segmentData.cKeyCode1);
                            col.ColumnName = dropdownOutputDto.Label = "Key Code 1";
                        }
                        else if (col.ColumnName == "ckeycode2")
                        {
                            foreach (DataRow dr in dumpDataTable.Rows)
                                dr.SetField("ckeycode2", segmentData.cKeyCode2);
                            col.ColumnName = dropdownOutputDto.Label = "Key Code 2";
                        }
                        else if (col.ColumnName == "idmsnumber")
                        {
                            foreach (DataRow dr in dumpDataTable.Rows)
                                dr.SetField("idmsnumber", campaignId);
                            col.ColumnName = dropdownOutputDto.Label = "IDMS#";
                        }
                        else
                        {
                            if (buildTableLayoutResult.Count(t => t.cFieldName == col.ColumnName) > 0)
                            {
                                var cFieldDescription = buildTableLayoutResult.FirstOrDefault(t => t.cFieldName == col.ColumnName).cFieldDescription;
                                col.ColumnName = dropdownOutputDto.Label = cFieldDescription;
                            }
                            if (isExportLayout && layoutExtObject.Count(t => t.cFieldName == col.ColumnName) > 0)
                            {
                                var cFieldDescription = layoutExtObject.FirstOrDefault(t => t.cFieldName == col.ColumnName).cFieldDescription;
                                col.ColumnName = dropdownOutputDto.Label = cFieldDescription;
                            }
                            else if (buildTableLayoutResult.Count(t => t.cFieldName == col.ColumnName) == 0)
                              dropdownOutputDto.Label = col.ColumnName;
                        }
                        segmentDataPreviewData.Columns.Add(dropdownOutputDto);
                    }
                    segmentDataPreviewData.DataForExport = dumpDataTable;
                    segmentDataPreviewData.Data = JsonConvert.SerializeObject(dumpDataTable);
                }
                return segmentDataPreviewData;
            }
            catch (Exception ex)
            {                
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Download
        public async Task<FileDto> DownloadDataPreview(int segmentId, bool isExportLayout)
        {
            try
            {
                var _fileName = $"SegmentDataPreview.xlsx";
                var _filePath = string.Format("{0}/{1}", _webRootPath, _fileName);
                var dataPreviewData = await GetRecordDumpAsync(segmentId, isExportLayout);
                var dtDump = dataPreviewData.DataForExport;
                //Create an instance of ExcelEngine
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    var application = excelEngine.Excel;
                    var workbook = application.Workbooks.Create(1);
                    workbook.Version = ExcelVersion.Excel2016;
                    var dataSheet = workbook.Worksheets[0];
                    dataSheet.Name = $"{L("DataPreviewTitle")}";
                    dataSheet.Range["A1"].Text = $"{L("DataPreviewTitle").ToUpper()}";
                    dataSheet.Range["A2"].Text = dataPreviewData.Description;
                    dataSheet.ImportDataTable(dtDump, true, 3, 1, -1, -1, true);
                    dataSheet.IsGridLinesVisible = false;
                    var row = dtDump.Rows.Count;
                    var col = dtDump.Columns.Count;
                    dataSheet.UsedRange[3, 1, row + 3, col].BorderInside();
                    dataSheet.UsedRange[3, 1, row + 3, col].BorderAround();
                    dataSheet.UsedRange[3, 1, 3, col].CellStyle.ColorIndex = ExcelKnownColors.Yellow;
                    dataSheet.UsedRange[3, 1, row + 3, col].AutofitColumns();
                        
                    using (FileStream fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        workbook.SaveAs(fileStream);
                        workbook.Close();
                    }
                    excelEngine.ThrowNotSavedOnDestroy = false;
                    var fileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet;
                    return new FileDto(_filePath, fileType,true);
                }
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion
        #endregion
    }
}
