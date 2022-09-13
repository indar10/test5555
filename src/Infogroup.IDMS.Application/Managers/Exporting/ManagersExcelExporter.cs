using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.Managers.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;
using OfficeOpenXml.Style;
using Abp.UI;
using System;

namespace Infogroup.IDMS.Managers.Exporting
{
    public class ManagersExcelExporter : EpPlusExcelExporterBase, IManagersExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ManagersExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<ContactAssignmentsDto> contactAssignments, string databaseName, string fileName)
        {
            try
            {

                return CreateExcelPackage(
                    $"{fileName}.xlsx",
                    excelPackage =>
                    {
                        var sheet = excelPackage.Workbook.Worksheets.Add(fileName);
                        sheet.OutLineApplyStyle = true;
                        var index = 1;
                        AddObject(
                             sheet, index, databaseName

                             );

                        index++;
                        AddHeader(
                          sheet,
                          index,
                         "List Manager",
                         "Contact Name",
                         "DWAP",
                         "List Order"
                            );

                        index++;

                        foreach (var item in contactAssignments)
                        {

                            var newList = new List<ContactAssignmentsDto>();
                            
                            newList.Add(item);

                            index = AddObjectsManager(
                                sheet, index, newList,
                                _ => _.ListManager,
                                _ => _.ContactName,
                                _ => _.Dwap,
                                _ => _.OrderList
                                );
                            index++;
                        }

                        var colFromHex = System.Drawing.ColorTranslator.FromHtml("#cad3df");
                        sheet.Cells[2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[2, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[2, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[2, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;

                        sheet.Cells[2, 1].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        sheet.Cells[2, 2].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        sheet.Cells[2, 3].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        sheet.Cells[2, 4].Style.Fill.BackgroundColor.SetColor(colFromHex);

                        sheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        var range = $"A3:A{index.ToString()}";
                        sheet.Cells[range].Style.WrapText = true;

                        sheet.Column(2).Style.WrapText = true;
                        sheet.Column(3).Style.WrapText = true;
                        sheet.Column(4).Style.WrapText = true;

                        sheet.Column(1).Width = 40;
                        sheet.Column(2).Width = 45;
                        sheet.Column(3).Width = 45;
                        sheet.Column(4).Width = 45;

                        var modelRange = $"A3:E{index.ToString()}";
                        sheet.View.ShowGridLines = false;

                    });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
    }
}
