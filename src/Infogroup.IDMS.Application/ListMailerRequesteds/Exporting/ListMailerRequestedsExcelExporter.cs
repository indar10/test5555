using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.ListMailerRequesteds.Exporting
{
    public class ListMailerRequestedsExcelExporter : EpPlusExcelExporterBase, IListMailerRequestedsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ListMailerRequestedsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetListMailerRequestedForViewDto> listMailerRequesteds)
        {
            return CreateExcelPackage(
                "ListMailerRequesteds.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("ListMailerRequesteds"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("MailerID"),
                        L("cCreatedBy"),
                        L("dCreatedDate"),
                        L("cModifiedBy"),
                        L("dModifiedDate"),
                        (L("MasterLoL")) + L("cListName")
                        );

                    AddObjects(
                        sheet, 2, listMailerRequesteds,
                        _ => _.ListMailerRequested.MailerID,
                        _ => _.ListMailerRequested.cCreatedBy,
                        _ => _timeZoneConverter.Convert(_.ListMailerRequested.dCreatedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.ListMailerRequested.cModifiedBy,
                        _ => _timeZoneConverter.Convert(_.ListMailerRequested.dModifiedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.MasterLoLcListName
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
