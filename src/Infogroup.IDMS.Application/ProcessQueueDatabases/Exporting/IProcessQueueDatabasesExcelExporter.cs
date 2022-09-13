using System.Collections.Generic;
using Infogroup.IDMS.ProcessQueueDatabases.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.ProcessQueueDatabases.Exporting
{
    public interface IProcessQueueDatabasesExcelExporter
    {
        FileDto ExportToFile(List<GetProcessQueueDatabaseForViewDto> processQueueDatabases);
    }
}