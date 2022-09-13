using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.BuildLoLs.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.BuildLoLs.Exporting
{
    public class BuildLolsExcelExporter : EpPlusExcelExporterBase, IBuildLolsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public BuildLolsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetBuildLolForViewDto> buildLols)
        {
            return CreateExcelPackage(
                "BuildLols.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("BuildLols"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("MasterLolID"),
                        L("LK_Action"),
                        L("LK_ActionMonth1"),
                        L("LK_ActionMonth2"),
                        L("LK_ActionNextMonth"),
                        L("LK_QuantityType"),
                        L("LK_FileType"),
                        L("iSkipFirstRow"),
                        L("iIsActive"),
                        L("iUsage"),
                        L("nTurns"),
                        L("cDecisionReasoning"),
                        L("cSlugDate"),
                        L("cBatchDateType"),
                        L("LK_SlugDateType"),
                        L("iQuantityPrevious"),
                        L("iQuantityRequested"),
                        L("iQuantityReceivedDP"),
                        L("iQuantityReceived"),
                        L("iQuantityConverted"),
                        L("dDateReceived"),
                        L("iQuantityTotal"),
                        L("cBatch_LastFROM"),
                        L("cBatch_LastTO"),
                        L("cBatch_FROM"),
                        L("cBatch_TO"),
                        L("Order_No"),
                        L("Order_ClientPO"),
                        L("OrderSelection"),
                        L("Order_Fields"),
                        L("Order_Comments"),
                        L("Order_Notes1"),
                        L("Order_Notes2"),
                        L("LK_EmailTemplate"),
                        L("ddateOrderSent"),
                        L("cNote"),
                        L("iCASApprovalTo"),
                        L("cSourceFilenameReadyToLoad"),
                        L("cSystemFilenameReadyToLoad"),
                        L("LK_LoadFileType"),
                        L("LK_LoadFileRowTerminator"),
                        L("cOnePassFileName"),
                        L("dCreatedDate"),
                        L("cCreatedBy"),
                        L("dModifiedDate"),
                        L("cModifiedBy"),
                        L("cSQL"),
                        L("cSQLDescription"),
                        L("iLoadQty"),
                        L("LK_Encoding"),
                        L("iIsMultiline"),
                        (L("Build")) + L("LK_BuildStatus")
                        );

                    AddObjects(
                        sheet, 2, buildLols,
                        _ => _.BuildLol.MasterLolID,
                        _ => _.BuildLol.LK_Action,
                        _ => _.BuildLol.LK_ActionMonth1,
                        _ => _.BuildLol.LK_ActionMonth2,
                        _ => _.BuildLol.LK_ActionNextMonth,
                        _ => _.BuildLol.LK_QuantityType,
                        _ => _.BuildLol.LK_FileType,
                        _ => _.BuildLol.iSkipFirstRow,
                        _ => _.BuildLol.iIsActive,
                        _ => _.BuildLol.iUsage,
                        _ => _.BuildLol.nTurns,
                        _ => _.BuildLol.cDecisionReasoning,
                        _ => _.BuildLol.cSlugDate,
                        _ => _.BuildLol.cBatchDateType,
                        _ => _.BuildLol.LK_SlugDateType,
                        _ => _.BuildLol.iQuantityPrevious,
                        _ => _.BuildLol.iQuantityRequested,
                        _ => _.BuildLol.iQuantityReceivedDP,
                        _ => _.BuildLol.iQuantityReceived,
                        _ => _.BuildLol.iQuantityConverted,
                        _ => _timeZoneConverter.Convert(_.BuildLol.dDateReceived, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.BuildLol.iQuantityTotal,
                        _ => _.BuildLol.cBatch_LastFROM,
                        _ => _.BuildLol.cBatch_LastTO,
                        _ => _.BuildLol.cBatch_FROM,
                        _ => _.BuildLol.cBatch_TO,
                        _ => _.BuildLol.Order_No,
                        _ => _.BuildLol.Order_ClientPO,
                        _ => _.BuildLol.OrderSelection,
                        _ => _.BuildLol.Order_Fields,
                        _ => _.BuildLol.Order_Comments,
                        _ => _.BuildLol.Order_Notes1,
                        _ => _.BuildLol.Order_Notes2,
                        _ => _.BuildLol.LK_EmailTemplate,
                        _ => _timeZoneConverter.Convert(_.BuildLol.ddateOrderSent, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.BuildLol.cNote,
                        _ => _.BuildLol.iCASApprovalTo,
                        _ => _.BuildLol.cSourceFilenameReadyToLoad,
                        _ => _.BuildLol.cSystemFilenameReadyToLoad,
                        _ => _.BuildLol.LK_LoadFileType,
                        _ => _.BuildLol.LK_LoadFileRowTerminator,
                        _ => _.BuildLol.cOnePassFileName,
                        _ => _timeZoneConverter.Convert(_.BuildLol.dCreatedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.BuildLol.cCreatedBy,
                        _ => _timeZoneConverter.Convert(_.BuildLol.dModifiedDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.BuildLol.cModifiedBy,
                        _ => _.BuildLol.cSQL,
                        _ => _.BuildLol.cSQLDescription,
                        _ => _.BuildLol.iLoadQty,
                        _ => _.BuildLol.LK_Encoding,
                        _ => _.BuildLol.iIsMultiline,
                        _ => _.BuildLK_BuildStatus
                        );

					var dDateReceivedColumn = sheet.Column(21);
                    dDateReceivedColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dDateReceivedColumn.AutoFit();
					var ddateOrderSentColumn = sheet.Column(35);
                    ddateOrderSentColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					ddateOrderSentColumn.AutoFit();
					var dCreatedDateColumn = sheet.Column(43);
                    dCreatedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dCreatedDateColumn.AutoFit();
					var dModifiedDateColumn = sheet.Column(45);
                    dModifiedDateColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dModifiedDateColumn.AutoFit();
					

                });
        }
    }
}
