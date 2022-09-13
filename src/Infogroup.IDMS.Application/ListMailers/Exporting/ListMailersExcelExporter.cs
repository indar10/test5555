using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.ListMailers.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.ListMailers.Exporting
{
    public class ListMailersExcelExporter : EpPlusExcelExporterBase, IListMailersExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ListMailersExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetListMailerForViewDto> listMailers)
        {
            return CreateExcelPackage(
                "ListMailers.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("ListMailers"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("ID"),
                        L("MailerID"),
                        L("dCreatedDate"),
                        L("cCreatedBy"),
                        L("cModifiedBy"),
                        L("dModifiedDate"),
                        (L("MasterLoL")) + L("cListName")
                        );

                    AddObjects(
                        sheet, 2, listMailers,
                        _ => _.ListMailer.ID,
                        _ => _.ListMailer.MailerID,
                        _ => _timeZoneConverter.Convert(_.ListMailer.dCreatedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.ListMailer.cCreatedBy,
                        _ => _.ListMailer.cModifiedBy,
                        _ => _timeZoneConverter.Convert(_.ListMailer.dModifiedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.MasterLoLcListName
                        );

					var dCreatedDateColumn = sheet.Column(3);
                    dCreatedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dCreatedDateColumn.AutoFit();
					var dModifiedDateColumn = sheet.Column(6);
                    dModifiedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dModifiedDateColumn.AutoFit();
					

                });
        }
    }
}
