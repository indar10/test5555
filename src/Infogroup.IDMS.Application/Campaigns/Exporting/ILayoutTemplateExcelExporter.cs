using System.Collections.Generic;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Campaigns.Exporting
{
    public interface ILayoutTemplateExcelExporter
    {
        FileDto ExportToFile(List<LayoutTemplateDto> campaigns);
    }
}