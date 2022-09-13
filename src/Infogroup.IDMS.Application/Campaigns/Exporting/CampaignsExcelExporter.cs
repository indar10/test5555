using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infogroup.IDMS.DataExporting.Excel.EpPlus;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.Campaigns.Exporting
{
    public class CampaignsExcelExporter : EpPlusExcelExporterBase, ICampaignsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CampaignsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }
        public FileDto ExportToFile(List<GetCampaignsListForView> Campaigns)
        {
            return CreateExcelPackage(
                "Campaigns.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Campaigns"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                      sheet,
                     L("ID"),
                     L("DatabaseName"),                      
                     L("cDescription"),
                     L("iProvidedQty"),
                     L("Status"),                     
                     L("dOrderCreatedDate")
                       
                        );

                    AddObjects(
                        sheet, 2, Campaigns,
                       _ => _.CampaignId,
                        _ => _.DatabaseName,
                        _ => _.CampaignDescription,
                        _ => _.OfferID,
                        _ => _.Status,
                        _=>_.OrderCreatedDate
           
                        );

                    var dDateLastRunColumn = sheet.Column(12);
                    dDateLastRunColumn.Style.Numberformat.Format = "yyyy-mm-dd";
                    dDateLastRunColumn.AutoFit();
                  


                });
        }

    }
}
