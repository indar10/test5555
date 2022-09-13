using Infogroup.IDMS.Databases.Dtos;
using Infogroup.IDMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Databases.Exporting
{
    public interface IDatabaseExcelExporter
    {
        FileDto ExportToFile(List<GetDatabaseAccessReportDto> FieldsTemplate, string databaseName);
    }
}
