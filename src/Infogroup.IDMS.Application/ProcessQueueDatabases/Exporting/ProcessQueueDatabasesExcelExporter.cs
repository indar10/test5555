using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.ProcessQueueDatabases.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.ProcessQueueDatabases.Exporting
{
    public class ProcessQueueDatabasesExcelExporter : EpPlusExcelExporterBase, IProcessQueueDatabasesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ProcessQueueDatabasesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetProcessQueueDatabaseForViewDto> processQueueDatabases)
        {
            return CreateExcelPackage(
                "ProcessQueueDatabases.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("ProcessQueueDatabases"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("DatabaseId"),
                        L("cCreatedBy"),
                        L("dCreatedDate"),
                        L("cModifiedBy"),
                        L("dModifiedDate"),
                        (L("ProcessQueue")) + L("cQueueName")
                        );

                    AddObjects(
                        sheet, 2, processQueueDatabases,
                        _ => _.ProcessQueueDatabase.DatabaseId,
                        _ => _.ProcessQueueDatabase.cCreatedBy,
                        _ => _timeZoneConverter.Convert(_.ProcessQueueDatabase.dCreatedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.ProcessQueueDatabase.cModifiedBy,
                        _ => _timeZoneConverter.Convert(_.ProcessQueueDatabase.dModifiedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.ProcessQueuecQueueName
                        );

					var dCreatedDateColumn = sheet.Column(3);
                    dCreatedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dCreatedDateColumn.AutoFit();
					var dModifiedDateColumn = sheet.Column(5);
                    dModifiedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dModifiedDateColumn.AutoFit();
					

                });
        }
    }
}
