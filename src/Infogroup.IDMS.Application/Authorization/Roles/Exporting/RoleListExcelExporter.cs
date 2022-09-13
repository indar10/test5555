using Abp.AspNetZeroCore.Net;
using Abp.Dependency;
using Infogroup.IDMS.Authorization.Roles.Dto;
using Infogroup.IDMS.Dto;
using Syncfusion.XlsIO;
using System.Collections.Generic;
using System.IO;

namespace Infogroup.IDMS.Authorization.Roles.Exporting
{
    public class RoleListExcelExporter: IDMSAppServiceBase, IRoleListExcelExporter, ITransientDependency
    {
        public FileDto ExportToFile(List<RoleReportDto> userReportDtos)
        {
            using(ExcelEngine excelEngine = new ExcelEngine())
            {
                var application = excelEngine.Excel;
                var workbook = application.Workbooks.Create(1);
                workbook.Version = ExcelVersion.Excel2016;
                var dataSheet = workbook.Worksheets[0];
                dataSheet.Name = $"{L("RoleSummaryReport")}";
                var fileName = $"{L("RoleSummaryReportFileName")}";

                var rows = userReportDtos.Count;
                dataSheet.ImportData(userReportDtos, 1, 1, true);
                if (rows > 0)
                {
                    dataSheet.Range[$@"A1:B{rows + 1}"].BorderAround(ExcelLineStyle.Thin);
                    dataSheet.Range[$@"A1:B{rows + 1}"].BorderInside(ExcelLineStyle.Thin);
                }
                dataSheet.Range[$"A1:B1"].CellStyle.Font.Bold = true;
                dataSheet.UsedRange.AutofitColumns();
                
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    workbook.SaveAs(fileStream);
                    workbook.Close();
                }

                var fileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet;
                return new FileDto(fileName, fileType, true);
            }
        }
    }
}
