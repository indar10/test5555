using System.Collections.Generic;
using Infogroup.IDMS.DivisionMailers.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.DivisionMailers.Exporting
{
    public interface IDivisionMailerExcelExporter
    {
        FileDto ExportToFile(List<DivisionMailerExportDto> divisionMailers, string fileName);
    }
}