using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.Lookups.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.Lookups.Exporting
{
    public class LookupsExcelExporter : EpPlusExcelExporterBase, ILookupsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public LookupsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetLookupForViewDto> lookups)
        {
            return CreateExcelPackage(
                "Lookups.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Lookups"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("ID"),
                        L("cLookupValue"),
                        L("iOrderBy"),
                        L("cCode"),
                        L("cDescription"),
                        L("cField"),
                        L("mField"),
                        L("iField"),
                        L("iIsActive"),
                        L("cCreatedBy"),
                        L("cModifiedBy"),
                        L("dCreatedDate"),
                        L("dModifiedDate")
                        );

                    AddObjects(
                        sheet, 2, lookups,
                        _ => _.Lookup.ID,
                        _ => _.Lookup.cLookupValue,
                        _ => _.Lookup.iOrderBy,
                        _ => _.Lookup.cCode,
                        _ => _.Lookup.cDescription,
                        _ => _.Lookup.cField,
                        _ => _.Lookup.mField,
                        _ => _.Lookup.iField,
                        _ => _.Lookup.iIsActive,
                        _ => _.Lookup.cCreatedBy,
                        _ => _.Lookup.cModifiedBy,
                        _ => _timeZoneConverter.Convert(_.Lookup.dCreatedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.Lookup.dModifiedDate, _abpSession.TenantId, _abpSession.GetUserId())
                        );

					var dCreatedDateColumn = sheet.Column(12);
                    dCreatedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dCreatedDateColumn.AutoFit();
					var dModifiedDateColumn = sheet.Column(13);
                    dModifiedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dModifiedDateColumn.AutoFit();
					

                });
        }
    }
}
