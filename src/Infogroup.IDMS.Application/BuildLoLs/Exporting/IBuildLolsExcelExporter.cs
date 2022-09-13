using System.Collections.Generic;
using Infogroup.IDMS.BuildLoLs.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.BuildLoLs.Exporting
{
    public interface IBuildLolsExcelExporter
    {
        FileDto ExportToFile(List<GetBuildLolForViewDto> buildLols);
    }
}