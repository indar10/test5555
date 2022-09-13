using System.Collections.Generic;
using Infogroup.IDMS.Managers.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Managers.Exporting
{
    public interface IManagersExcelExporter
    {
        FileDto ExportToFile(List<ContactAssignmentsDto> managers, string databaseName, string fileName);
    }
}