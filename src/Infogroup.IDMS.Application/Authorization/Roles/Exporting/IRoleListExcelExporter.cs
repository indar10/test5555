using Infogroup.IDMS.Authorization.Roles.Dto;
using Infogroup.IDMS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Authorization.Roles.Exporting
{
    public interface IRoleListExcelExporter
    {
        FileDto ExportToFile(List<RoleReportDto> roleReportDto);
    }
}
