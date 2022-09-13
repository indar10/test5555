using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.Campaigns.Exporting
{
    public class LayoutTemplateExcelExporter : EpPlusExcelExporterBase, ILayoutTemplateExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public LayoutTemplateExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }
        public FileDto ExportToFile(List<LayoutTemplateDto> layoutTemplate)
        {
            return CreateExcelPackage(
                "ExportOrderLayout.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("ExportOrderLayout"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                      sheet,
                     "Order",
                     "Field Description",
                     "Field/Formula",
                     "Width"
                    

                        );

                    AddObjects(
                        sheet, 2, layoutTemplate,
                       _ => _.Order,
                        _ => _.FieldName,
                        _ => _.Formula,
                        _ => _.Width
                        

                        );

                    var dDateLastRunColumn = sheet.Column(12);
                    dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                    dDateLastRunColumn.AutoFit();



                });
        }

    }
}
