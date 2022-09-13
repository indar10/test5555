using System.Collections.Generic;
using Infogroup.IDMS.Auditing.Dto;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}
