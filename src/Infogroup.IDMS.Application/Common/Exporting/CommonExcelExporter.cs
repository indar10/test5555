using Abp.AspNetZeroCore.Net;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Abp.UI;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.GroupBrokers.Dtos;
using Infogroup.IDMS.Owners.Dtos;
using Infogroup.IDMS.SecurityGroups.Dtos;
using Infogroup.IDMS.SelectionFieldCountReports.Dtos;
using Infogroup.IDMS.Storage;
using OfficeOpenXml.Style;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Infogroup.IDMS.IDMSConfigurations;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Infogroup.IDMS.MasterLoLs.Dtos;
using Infogroup.IDMS.ExternalBuildTableDatabases.Dtos;

namespace Infogroup.IDMS.Common.Exporting
{
    public class CommonExcelExporter : EpPlusExcelExporterBase, ICommonExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        public CommonExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager, IRedisIDMSConfigurationCache idmsConfigurationCache) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
            _idmsConfigurationCache = idmsConfigurationCache;
        }

        public FileDto AllExportToFile(List<ExcelExporterDto> excelDetails, string databaseName, string fileName)
        {
            try
            {

                return CreateExcelPackage(
                    $"{fileName}.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add(L(fileName));
                        sheet.OutLineApplyStyle = true;


                        AddObject(
                             sheet, 1, databaseName

                             );

                        

                        AddHeader(
                          sheet,
                          3,
                         "Company",
                         "Address"
                            );

                        var index = 4;

                        foreach (var item in excelDetails)
                        {
                            var newList = new List<ExcelExporterDto>();
                            newList.Add(item);

                            AddObjects(
                                sheet, index, newList,
                               _ => _.cCompany,
                                _ => _.cAddress = GenerateAddress(item)
                                );


                            index += 2;
                            AddHeader(
                              sheet,
                              index,
                              2,
                             "Name",
                             "Address",
                             "Phone",
                             "Email"
                                );

                            var colFromHexa = System.Drawing.ColorTranslator.FromHtml("#cad3df");

                            sheet.Cells[index, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sheet.Cells[index, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sheet.Cells[index, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sheet.Cells[index, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;

                            sheet.Cells[index, 2].Style.Fill.BackgroundColor.SetColor(colFromHexa);
                            sheet.Cells[index, 3].Style.Fill.BackgroundColor.SetColor(colFromHexa);
                            sheet.Cells[index, 4].Style.Fill.BackgroundColor.SetColor(colFromHexa);
                            sheet.Cells[index, 5].Style.Fill.BackgroundColor.SetColor(colFromHexa);



                            index += 1;
                            AddObjects(
                                sheet, index, 1, item.ContactsList,
                               _ => _.cFirstName + " " + _.cLastName,
                                _ => _.cAddress1,                      
                                _ => GeneratePhoneAddress("",_.cPhone1,_.cFax),
                                _ => _.cEmailAddress
                                );

                            index = index + item.ContactsList.Count + 1;
                        }


                        var colFromHex = System.Drawing.ColorTranslator.FromHtml("#cad3df");
                        sheet.Cells[3, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;

                        sheet.Cells[3, 1].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        sheet.Cells[3, 2].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        var range = $"A3:A{index.ToString()}";
                        sheet.Cells[range].Style.WrapText = true;

                        sheet.Column(2).Style.WrapText = true;
                        sheet.Column(3).Style.WrapText = true;

                        sheet.Column(1).Width = 35;
                        sheet.Column(2).Width = 40;
                        sheet.Column(3).Width = 40;
                        sheet.Column(4).AutoFit();
                        sheet.Column(5).AutoFit();

                        var modelRange = $"A3:E{index.ToString()}";

                        var modelTable = sheet.Cells[modelRange];
                        modelTable.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        
                        sheet.View.ShowGridLines = false;

                        var dDateLastRunColumn = sheet.Column(12);
                        dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                        dDateLastRunColumn.AutoFit();



                    });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public FileDto AllExportToFileForMasterLol(List<ExportToExcelMasterLolDto> excelDetails, string databaseName, string fileName)
        {
            try
            {


                return CreateExcelPackage(
                    $"{fileName}.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add(L(fileName));
                        sheet.OutLineApplyStyle = true;


                        AddObject(
                             sheet, 1, databaseName

                             );

                        AddHeader(
                          sheet,
                          3,
                         "ID",
                         "List Name",
                         "Manager Name",
                         "Decision Group",
                         "List Owner",
                         "Permission Type",
                         "List Type",
                         "Product Code",
                         "Code",
                         "Min Code",
                         "Next Mark ID",
                         "$ Postal",
                         "$ Tele Mktg.",
                         "Appear Date",
                         "Update Date",
                         "Remove Date",
                         "Send Order To",
                         "MultiBuyer",
                         "Drop Dups",
                         "Send Dwap To",
                         "Active",
                         "Commission",
                         "A/R Fee",
                         "Legacy Code",
                         "Reason Code",
                         "Cap Offer 1",
                         "Cap Offer 2",
                         "Cap Offer 3",
                         "Cap Offer 4",
                         "Number",
                         "Basic Selects",
                         "Dwap Contacts",
                         "Requested By Mailer",
                         "Available Mailer"
                            );

                        var index = 4;

                        foreach (var item in excelDetails)
                        {
                            var newList = new List<ExportToExcelMasterLolDto>();
                            newList.Add(item);

                            AddObjects(
                                sheet, index, newList,
                               _ => _.ID,
                                _ => _.cListName,
                                 _ => _.ManagerName,
                                 _ => _.DecisionGroup,
                                 _ => _.ListOwner,
                                  _ => _.LK_PermissionType,
                                  _ => _.ListType,
                                  _ => _.productCode,
                                   _ => _.cCode,
                                   _ => _.cMINDatacardCode,
                                   _ => _.cNextMarkID,
                                    _ => _.nBasePrice_Postal,
                                     _ => _.nBasePrice_Telemarketing,
                                     _ => _.cAppearDate,
                                     _ => _.cLastUpdateDate,
                                     _ => _.cRemoveDate,
                                     _ => _.SendOrderTo,
                                     _ => _.MultiBuyer,
                                     _ => _.dD,
                                     _ => _.sendDwap,
                                      _ => _.Active,
                                      _ => _.cCustomText1,
                                      _ => _.cCustomText2,
                                      _ => _.cCustomText3,
                                      _ => _.cCustomText4,
                                      _ => _.cCustomText5,
                                      _ => _.cCustomText6,
                                      _ => _.cCustomText7,
                                      _ => _.cCustomText8,
                                      _ => _.cCustomText9,
                                      _ => _.cCustomText10,
                                      _ => _.DwapContacts,
                                      _ => _.ReqMailer,
                                      _ => _.AvailableMailer
                                );

                            index += 1;
                         

                        }


                        var colFromHex = System.Drawing.ColorTranslator.FromHtml("#cad3df");
                        for(var i = 1; i <= 34; i++)
                        {
                            sheet.Cells[3, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        }
                        for (var i = 1; i <= 34; i++)
                        {
                            sheet.Cells[3, i].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        sheet.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(11).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(12).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(13).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(14).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(15).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(16).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(17).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(18).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(19).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(20).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(21).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(22).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(23).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(24).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(25).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(26).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(27).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(28).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(29).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(30).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(31).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(32).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(33).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(34).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        sheet.Column(1).Width = 10;
                        sheet.Column(2).AutoFit();
                        sheet.Column(3).AutoFit();
                        sheet.Column(4).AutoFit();
                        sheet.Column(5).AutoFit();
                        sheet.Column(6).AutoFit();
                        sheet.Column(7).AutoFit();
                        sheet.Column(8).AutoFit();
                        sheet.Column(9).AutoFit();
                        sheet.Column(10).AutoFit();
                        sheet.Column(11).AutoFit();
                        sheet.Column(12).AutoFit();
                        sheet.Column(13).AutoFit();
                        sheet.Column(14).AutoFit();
                        sheet.Column(15).AutoFit();
                        sheet.Column(16).AutoFit();
                        sheet.Column(17).AutoFit();
                        sheet.Column(18).AutoFit();
                        sheet.Column(19).AutoFit();
                        sheet.Column(20).AutoFit();
                        sheet.Column(21).AutoFit();
                        sheet.Column(22).AutoFit();
                        sheet.Column(23).AutoFit();
                        sheet.Column(24).AutoFit();
                        sheet.Column(25).AutoFit();
                        sheet.Column(26).AutoFit();
                        sheet.Column(27).AutoFit();
                        sheet.Column(28).AutoFit();
                        sheet.Column(29).AutoFit();
                        sheet.Column(30).AutoFit();
                        sheet.Column(31).AutoFit();
                        sheet.Column(32).AutoFit();
                        sheet.Column(33).AutoFit();
                        sheet.Column(34).AutoFit();


                        sheet.View.ShowGridLines = true;

                        var dDateLastRunColumn = sheet.Column(16);
                        dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                        dDateLastRunColumn.AutoFit();



                    });

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public FileDto AllExportToFileForListMailerAccess(List<ExportListMailerAccess> excelDetails, string databaseName, string fileName)
        {
            try
            {


                return CreateExcelPackage(
                    $"{fileName}.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add(L(fileName));
                        sheet.OutLineApplyStyle = true;


                        AddObject(
                             sheet, 1, databaseName

                             );

                        AddHeader(
                          sheet,
                          3,
                         "List ID",
                         "Code",
                         "List Name",
                         "Type",
                         "Company"
                            );

                        var index = 4;

                        foreach (var item in excelDetails)
                        {
                            var newList = new List<ExportListMailerAccess>();
                            newList.Add(item);

                            AddObjects(
                                sheet, index, newList,
                               _ => _.ListId,
                                _ => _.Code,
                                 _ => _.ListName,
                                 _ => _.Type,
                                 _ => _.Company  
                                );

                            index += 1;


                        }


                        var colFromHex = System.Drawing.ColorTranslator.FromHtml("#cad3df");
                        for (var i = 1; i <= 5; i++)
                        {
                            sheet.Cells[3, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        }
                        for (var i = 1; i <= 5; i++)
                        {
                            sheet.Cells[3, i].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        sheet.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                      

                        sheet.Column(1).Width = 10;
                        sheet.Column(2).AutoFit();
                        sheet.Column(3).AutoFit();
                        sheet.Column(4).AutoFit();
                        sheet.Column(5).AutoFit();
                    

                        sheet.View.ShowGridLines = true;

                        var dDateLastRunColumn = sheet.Column(12);
                        dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                        dDateLastRunColumn.AutoFit();



                    });

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public FileDto AllExportToExternalDatabaseLinks(List<ExternalBuildTableDatabaseForAllDto> excelDetails,string fileName)
        {
            try
            {


                return CreateExcelPackage(
                    $"{fileName}.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add(L(fileName));
                        sheet.OutLineApplyStyle = true;

                        AddHeader(
                          sheet,
                          1,
                         "ID",
                         "Division Name",
                         "Database Name",
                         "Table Description",
                         "Table Name"
                            );

                        var index = 2;

                        foreach (var item in excelDetails)
                        {
                            var newList = new List<ExternalBuildTableDatabaseForAllDto>();
                            newList.Add(item);

                            AddObjects(
                                sheet, index, newList,
                               _ => _.ID,
                                _ => _.DivisionName,
                                 _ => _.DatabaseName,
                                 _ => _.BuildTableDescription,
                                 _ => _.BuildTableName
                                );

                            index += 1;


                        }


                        var colFromHex = System.Drawing.ColorTranslator.FromHtml("#cad3df");
                        for (var i = 1; i <= 5; i++)
                        {
                            sheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        }
                        for (var i = 1; i <= 5; i++)
                        {
                            sheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                        sheet.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;


                        sheet.Column(1).Width = 10;
                        sheet.Column(2).AutoFit();
                        sheet.Column(3).AutoFit();
                        sheet.Column(4).AutoFit();
                        sheet.Column(5).AutoFit();


                        sheet.View.ShowGridLines = false;

                        var dDateLastRunColumn = sheet.Column(12);
                        dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                        dDateLastRunColumn.AutoFit();



                    });

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public FileDto SelectionFieldCountAllExportToFile(List<GetSelectionFieldCountReportView> excelDetails, string databaseName, string fileName)
        {
            try
            {

                return CreateExcelPackage(
                    $"{fileName}.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add(L(fileName));
                        sheet.OutLineApplyStyle = true;


                        AddObject(
                             sheet, 1, databaseName

                             );



                        AddHeader(
                          sheet,
                          3,
                         "FieldName",
                         "Status"
                            );

                        var index = 5;

                        foreach (var item in excelDetails)
                        {
                            var newList = new List<GetSelectionFieldCountReportView>();
                            newList.Add(item);

                            AddObjects(
                                sheet, index, newList,
                               _ => _.cQuestionFieldName,
                                _ => _.cDescription
                                );


                            index += 2;
                            AddHeader(
                              sheet,
                              index,
                              2,
                             "Order ID",
                             "Description",
                             "Provided Qty.",
                             "Status Date",
                             "Created By"
                                );

                            var colFromHexa = System.Drawing.ColorTranslator.FromHtml("#cad3df");

                            sheet.Cells[index, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sheet.Cells[index, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sheet.Cells[index, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sheet.Cells[index, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            sheet.Cells[index, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;

                            sheet.Cells[index, 2].Style.Fill.BackgroundColor.SetColor(colFromHexa);
                            sheet.Cells[index, 3].Style.Fill.BackgroundColor.SetColor(colFromHexa);
                            sheet.Cells[index, 4].Style.Fill.BackgroundColor.SetColor(colFromHexa);
                            sheet.Cells[index, 5].Style.Fill.BackgroundColor.SetColor(colFromHexa);
                            sheet.Cells[index, 6].Style.Fill.BackgroundColor.SetColor(colFromHexa);



                            index += 1;
                            AddObjects(
                                sheet, index, 1, item.OrderDetailsList,
                               _ => _.OrderId ,
                                _ => _.cDescription,
                                _ => _.iProvidedCount,
                                _ => _.dCreatedDate,
                                _ =>_.CreatedBy
                                );

                            index = index + item.OrderDetailsList.Count + 1;
                        }


                        var colFromHex = System.Drawing.ColorTranslator.FromHtml("#cad3df");
                        sheet.Cells[3, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;

                        sheet.Cells[3, 1].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        sheet.Cells[3, 2].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        var range = $"A3:A{index.ToString()}";
                        sheet.Cells[range].Style.WrapText = true;                        

                        sheet.Column(2).Style.WrapText = true;
                        sheet.Column(3).Style.WrapText = true;
                        sheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;                       
                        sheet.Column(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        sheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;


                        //sheet.Column(1).Width = 35;
                        //sheet.Column(2).Width = 40;
                        //sheet.Column(3).Width = 40;
                        //sheet.Column(4).AutoFit();
                        //sheet.Column(5).AutoFit();
                        //sheet.Column(6).AutoFit();


                        sheet.Column(1).Width = 35;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 40;
                        sheet.Column(4).Width = 30;
                        sheet.Column(5).Width = 30;
                        sheet.Column(6).Width = 40;

                        var modelRange = $"A3:F{index.ToString()}";

                        var modelTable = sheet.Cells[modelRange];
                        modelTable.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        sheet.View.ShowGridLines = false;

                        var dDateLastRunColumn = sheet.Column(12);
                        dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                        dDateLastRunColumn.AutoFit();



                    });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public FileDto FastCountReportAllExportToFile(DataSet dataSet)
        {
            try
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    var application = excelEngine.Excel;
                    var workbook = application.Workbooks.Create(dataSet.Tables.Count);
                    workbook.Version = ExcelVersion.Excel2016;
                    int worksheetno = 0;
                    var fileName = "FastCountReport.xlsx";
                    var idmsConfigurationValues = JObject.Parse(_idmsConfigurationCache.GetConfigurationValue("FastCount", 1267).mValue);
                    foreach (DataTable item in dataSet.Tables)
                    {
                        var dataSheet = workbook.Worksheets[worksheetno];                      
                        item.Columns.Remove("id");
                        item.Columns.Remove("rowOrder");
                        item.Columns.Remove("Segment");
                        item.Columns.Remove("rowcount");
                        string sheetName = ((worksheetno + 1) + "." + textInfo.ToTitleCase(item.Columns[0].ColumnName));
                        if (item.Columns.Count > 2)
                        {
                            sheetName = ((worksheetno + 1) + "." + textInfo.ToTitleCase(item.Columns[0].ColumnName) + "_" + textInfo.ToTitleCase(item.Columns[1].ColumnName));
                        }
                        dataSheet.Name = sheetName.Length < 30 ? sheetName : sheetName.Substring(0, 30);
                        for (int i = 0; i < item.Columns.Count; i++)
                        {
                            item.Columns[i].ColumnName = textInfo.ToTitleCase(GetDescription(idmsConfigurationValues, item.Columns[i].ColumnName));
                        }
                        dataSheet.ImportDataTable(item, true, 1, 1);                    
                        dataSheet.UsedRange.BorderInside(ExcelLineStyle.Thin);
                        dataSheet.UsedRange.BorderAround(ExcelLineStyle.Thin);
                        dataSheet.UsedRange.Rows[0].CellStyle.Font.Bold = true;
                        dataSheet.UsedRange.Columns[item.Columns.Count -1 ].NumberFormat= "#,##0";
                        dataSheet.UsedRange.AutofitColumns();
                        worksheetno++;
                    }
                   using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        workbook.SaveAs(fileStream);
                        workbook.Close();
                    }

                    var fileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet;
                    return new FileDto(fileName, fileType, true);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public string GetDescription(JObject idmsConfigurationValues, string field)
        {
            string description = field;
            if (idmsConfigurationValues != null)
            {
                var ds = (idmsConfigurationValues["selection-fields"] as JObject).GetValue(field, StringComparison.OrdinalIgnoreCase);
                if (ds != null)
                {
                    description = Convert.ToString(ds["fc-description"]);
                }
            }
            return description==null ? field : description;
        }

        private static string GenerateAddress(ExcelExporterDto owner)
        {
            var address = string.Empty;
            if (!string.IsNullOrWhiteSpace(owner.cAddress1))
                address = owner.cAddress1;
            if (!string.IsNullOrWhiteSpace(owner.cAddress2))
                address = string.IsNullOrWhiteSpace(address) ? owner.cAddress2 : $"{address}, {owner.cAddress2}";
            if (!string.IsNullOrWhiteSpace(owner.cCity))
                address = string.IsNullOrWhiteSpace(address) ? owner.cCity : $"{address}, {owner.cCity}";
            if (!string.IsNullOrWhiteSpace(owner.cState))
                address = string.IsNullOrWhiteSpace(address) ? owner.cState : $"{address}, {owner.cState}";
            if (!string.IsNullOrWhiteSpace(owner.cZip))
                address = string.IsNullOrWhiteSpace(address) ? owner.cZip : $"{address}, {owner.cZip}";
           address = GeneratePhoneAddress(address, owner.cPhone, owner.cFax);
            return address;
        }
        private static string GeneratePhoneAddress(string address,string cPhone,string cFax)
        {
            if (!string.IsNullOrWhiteSpace(cPhone))
                address = string.IsNullOrWhiteSpace(address) ? (!cPhone.Contains('P') ? $"P: {cPhone}" : cPhone) : (!cPhone.Contains('P') ? $" {address} P: {cPhone}" : $" {address} {cPhone}");
            if (!string.IsNullOrWhiteSpace(cFax))
            {
                if (!string.IsNullOrWhiteSpace(cPhone))
                    address = !cFax.Contains('F') ? $"{address} / F: {cFax}" : $"{address} / {cFax}";
                else
                    address = string.IsNullOrWhiteSpace(address) ? (!cFax.Contains('F') ? $"F: {cFax}" : cFax) : (!cFax.Contains('F') ? $" {address} F: {cFax}" : $" {address} {cFax}");
            }
            return address;

        }
    }
}
