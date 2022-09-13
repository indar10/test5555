using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.Databases.Dtos;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Infogroup.IDMS.Databases.Exporting
{
    public class DatabaseExcelExporter : EpPlusExcelExporterBase, IDatabaseExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public DatabaseExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }
        public FileDto ExportToFile(List<GetDatabaseAccessReportDto> FieldsTemplate, string databaseName)
        {
            return CreateExcelPackage(
                "DatabaseAccessReport.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("DatabaseAccessReport"));
                    sheet.OutLineApplyStyle = true;


                    AddObject(
                         sheet, 1, databaseName



                         );
                    

                    AddHeader(
                      sheet,
                      3,
                     "First Name",
                     "Last Name",
                     "User ID",
                     "Email Address"
                        );

                    AddObjects(
                        sheet, 4, FieldsTemplate,
                       _ => _.FirstName,
                        _ => _.LastName,
                        _ => _.UserID,
                        _ => _.Email


                        );
                    var colFromHex = System.Drawing.ColorTranslator.FromHtml("#ffff00");
                    sheet.Cells[3, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[3, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[3, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                
                    sheet.Cells[3, 1].Style.Fill.BackgroundColor.SetColor(colFromHex);
                    sheet.Cells[3, 2].Style.Fill.BackgroundColor.SetColor(colFromHex);
                    sheet.Cells[3, 3].Style.Fill.BackgroundColor.SetColor(colFromHex);
                    sheet.Cells[3, 4].Style.Fill.BackgroundColor.SetColor(colFromHex);

                    var modelCells = sheet.Cells["A3"];
                    var modelRows = FieldsTemplate.Count() + 3;
                    var modelRange = "A3:D" + modelRows.ToString();
                    var modelTable = sheet.Cells[modelRange];

                    modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;                  

                    sheet.Cells[3, 1].AutoFitColumns();
                    sheet.Column(2).AutoFit();
                    sheet.Column(3).AutoFit();
                    sheet.Column(4).AutoFit();

                    sheet.View.ShowGridLines = false;

                    var dDateLastRunColumn = sheet.Column(12);
                    dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                    dDateLastRunColumn.AutoFit();



                });
        }
    }
}
