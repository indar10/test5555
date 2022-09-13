using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.DivisionMailers.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;
using OfficeOpenXml.Style;

namespace Infogroup.IDMS.DivisionMailers.Exporting
{
    public class DivisionMailerExcelExporter : EpPlusExcelExporterBase, IDivisionMailerExcelExporter
    {

        public DivisionMailerExcelExporter(
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
        }
        public FileDto ExportToFile(List<DivisionMailerExportDto> divisionMailers, string fileName)
        {
            return CreateExcelPackage(
                 $"{fileName}{L("excelFileExtension")}",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(fileName);
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                      sheet,
                     L("Company"),
                     L("Address")
                        );

                    AddObjects(
                        sheet, 2, divisionMailers,
                       _ => _.Company,
                        _ => _.Address = GenerateAddress(_.cAddr1, _.cAddr2, _.cCity, _.cState, _.cPhone, _.cFax, _.cZip)

                        );


                    var colFromHex = System.Drawing.ColorTranslator.FromHtml("#cad3df");
                    sheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    sheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(colFromHex);
                    sheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(colFromHex);

                    sheet.Column(1).Style.WrapText = true;
                    sheet.Column(2).Style.WrapText = true;

                    sheet.Column(1).Width = 80;
                    sheet.Column(2).Width = 80;

                    var modelRange = $"A1:B{(divisionMailers.Count() + 1).ToString()}";

                    var modelTable = sheet.Cells[modelRange];
                    modelTable.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    sheet.View.ShowGridLines = false;

                    var dDateLastRunColumn = sheet.Column(12);
                    dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                    dDateLastRunColumn.AutoFit();
                });
        }

        private static string GenerateAddress(string cAddr1, string cAddr2, string cCity, string cState, string cPhone, string cFax, string cZip)
        {
            var address = string.Empty;
            if (!string.IsNullOrWhiteSpace(cAddr1))
                address = cAddr1;
            if (!string.IsNullOrWhiteSpace(cAddr2))
                address = string.IsNullOrWhiteSpace(address) ? cAddr2 : $"{address}, {cAddr2}";
            if (!string.IsNullOrWhiteSpace(cCity))
                address = string.IsNullOrWhiteSpace(address) ? cCity : $"{address}, {cCity}";
            if (!string.IsNullOrWhiteSpace(cState))
                address = string.IsNullOrWhiteSpace(address) ? cState : $"{address}, {cState}";
            if (!string.IsNullOrWhiteSpace(cZip))
                address = string.IsNullOrWhiteSpace(address) ? cZip : $"{address}, {cZip}";
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
