using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.ProcessQueues.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.ProcessQueues.Exporting
{
    public class ProcessQueuesExcelExporter : EpPlusExcelExporterBase, IProcessQueuesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ProcessQueuesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetProcessQueueForViewDto> processQueues)
        {
            return CreateExcelPackage(
                "ProcessQueues.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("ProcessQueues"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("cQueueName"),
                        L("cDescription"),
                        L("iAllowedThreadCount"),
                        L("LK_QueueType"),
                        L("LK_ProcessType"),
                        L("iIsSuspended"),
                        L("dCreatedDate"),
                        L("cCreatedBy"),
                        L("dModifiedDate"),
                        L("cModifiedBy")
                        );

                    AddObjects(
                        sheet, 2, processQueues,
                        _ => _.ProcessQueue.cQueueName,
                        _ => _.ProcessQueue.cDescription,
                        _ => _.ProcessQueue.iAllowedThreadCount,
                        _ => _.ProcessQueue.LK_QueueType,
                        _ => _.ProcessQueue.LK_ProcessType,
                        _ => _.ProcessQueue.iIsSuspended,
                        _ => _timeZoneConverter.Convert(_.ProcessQueue.dCreatedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.ProcessQueue.cCreatedBy,
                        _ => _timeZoneConverter.Convert(_.ProcessQueue.dModifiedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.ProcessQueue.cModifiedBy
                        );

					var dCreatedDateColumn = sheet.Column(7);
                    dCreatedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dCreatedDateColumn.AutoFit();
					var dModifiedDateColumn = sheet.Column(9);
                    dModifiedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dModifiedDateColumn.AutoFit();
					

                });
        }
    }
}
