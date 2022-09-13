using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.Mailers.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;
using OfficeOpenXml.Style;

namespace Infogroup.IDMS.Mailers.Exporting
{
    public class MailersExcelExporter : EpPlusExcelExporterBase, IMailersExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public MailersExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<MailerOfferDto> mailerOffers, string databaseName, string fileName)
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
                        sheet, 2,
                        "Mailer",
                        "Offer",
                        "Code",
                        "Type",
                        "Hidden"
                        );

                    AddObjects(
                        sheet, 3, mailerOffers,
                        _ => _.cCompany,
                        _ => _.cOfferName,
                        _ => _.cOfferCode,
                        _ => _.LK_OfferType,
                        _ => _.iHideInDWAP
                        );

                    var colFromHex = System.Drawing.ColorTranslator.FromHtml("#ffff00");
                    sheet.Cells[2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[2, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[2, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[2, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[2, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    sheet.Cells[2, 1].Style.Fill.BackgroundColor.SetColor(colFromHex);
                    sheet.Cells[2, 2].Style.Fill.BackgroundColor.SetColor(colFromHex);
                    sheet.Cells[2, 3].Style.Fill.BackgroundColor.SetColor(colFromHex);
                    sheet.Cells[2, 4].Style.Fill.BackgroundColor.SetColor(colFromHex);
                    sheet.Cells[2, 5].Style.Fill.BackgroundColor.SetColor(colFromHex);

                    var range = $"A2:A{(mailerOffers.Count + 2).ToString()}";
                    sheet.Cells[range].Style.WrapText = true;
                    sheet.Column(2).Style.WrapText = true;

                    sheet.Column(1).Width = 40;
                    sheet.Column(2).Width = 40;
                    sheet.Column(3).AutoFit();
                    sheet.Column(4).AutoFit();
                    sheet.Column(5).AutoFit();

                    var modelRows = mailerOffers.Count + 2;
                    var modelRange = $"A2:E{modelRows.ToString()}";

                    var modelTable = sheet.Cells[modelRange];
                    modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    sheet.View.ShowGridLines = false;


                });
        }
    }
}
