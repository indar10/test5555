using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;
using Infogroup.IDMS.ExportLayouts.Dtos;

namespace Infogroup.IDMS.ExportLayouts.Exporting
{
    public class LayoutExcelExporter : EpPlusExcelExporterBase, ILayoutExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public LayoutExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }
        public FileDto ExportToFile(List<ExportLayoutTemplateDto> FieldsTemplate, List<ExportLayoutTemplateDto> layoutTemplate, string databaseName, string buildId)
        {
            return CreateExcelPackage(
                "OutputLayout.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Output Layout"));
                    sheet.OutLineApplyStyle = true;                    
                   

                    AddObject(
                         sheet, 1, databaseName
                                               


                         );
                    AddObject(
                        sheet, 2, buildId



                        );

                    AddHeader(
                      sheet,
                      4,
                     "Description",
                     "Group Name",
                     "Telemarketing",
                     "OutputCase"
                        );

                    AddObjects(
                        sheet, 5, layoutTemplate,
                       _ => _.Description,
                        _ => _.GroupName,
                        _ => _.Telemarketing,
                        _ => _.OutputCase
                      

                        );

                    AddHeader(
                      sheet,
                      7,
                     "Order",
                     "Field Description",
                     "Field/Formula",
                     "Width",
                     "Table Name",
                     "Table Description"
                    

                        );

                    AddObjects(
                        sheet, 8, FieldsTemplate,
                       _ => _.Order,
                        _ => _.OutputFieldName,
                        _ => _.Formula,
                        _ => _.Width,
                        _=>_.TableName,
                        _=>_.TableDescription
                        

                        );

                    var dDateLastRunColumn = sheet.Column(12);
                    dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                    dDateLastRunColumn.AutoFit();



                });
        }

    }
}
