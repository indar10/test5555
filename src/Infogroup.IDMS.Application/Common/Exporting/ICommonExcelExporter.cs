using Infogroup.IDMS.Dto;
using Infogroup.IDMS.ExternalBuildTableDatabases.Dtos;
using Infogroup.IDMS.GroupBrokers.Dtos;
using Infogroup.IDMS.MasterLoLs.Dtos;
using Infogroup.IDMS.Owners.Dtos;
using Infogroup.IDMS.SecurityGroups.Dtos;
using Infogroup.IDMS.SelectionFieldCountReports.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infogroup.IDMS.Common.Exporting
{
    public interface ICommonExcelExporter
    {
        FileDto AllExportToFile(List<ExcelExporterDto> excelDetails, string databaseName, string fileName);
        FileDto SelectionFieldCountAllExportToFile(List<GetSelectionFieldCountReportView> excelDetails, string databaseName, string fileName);
        FileDto FastCountReportAllExportToFile(DataSet dataSet);

        FileDto AllExportToFileForMasterLol(List<ExportToExcelMasterLolDto> excelDetails, string databaseName, string fileName);
        
        FileDto AllExportToFileForListMailerAccess(List<ExportListMailerAccess> excelDetails, string databaseName, string fileName);

        FileDto AllExportToExternalDatabaseLinks(List<ExternalBuildTableDatabaseForAllDto> excelDetails, string fileName);
    }
}
