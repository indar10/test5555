using System.Collections.Generic;
using Infogroup.IDMS.Lookups.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Lookups.Exporting
{
    public interface ILookupsExcelExporter
    {
        FileDto ExportToFile(List<GetLookupForViewDto> lookups);
    }
}