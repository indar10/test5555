using System.Collections.Generic;
using Infogroup.IDMS.ProcessQueues.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.ProcessQueues.Exporting
{
    public interface IProcessQueuesExcelExporter
    {
        FileDto ExportToFile(List<GetProcessQueueForViewDto> processQueues);
    }
}