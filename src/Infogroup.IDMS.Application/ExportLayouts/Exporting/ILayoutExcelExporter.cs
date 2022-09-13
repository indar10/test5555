using System.Collections.Generic;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.ExportLayouts.Dtos;

namespace Infogroup.IDMS.ExportLayouts.Exporting
{
    public interface ILayoutExcelExporter
    {
        FileDto ExportToFile(List<ExportLayoutTemplateDto> FieldsTemplate, List<ExportLayoutTemplateDto> layoutTemplate, string database, string build);
    }
}