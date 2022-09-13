using Abp.Domain.Repositories;
using Infogroup.IDMS.BuildTables.Dtos;
using Infogroup.IDMS.ExportLayouts.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infogroup.IDMS.BuildTables
{
    public interface IBuildTableRepository : IRepository<BuildTable, int>
    {
        List<BuildTableDto> GetExternalTables(int campaignId);
        List<GetExportLayoutAddFieldsDto> GetExportLayoutAddFields(int tableId, int campaignId);
        List<BuildTableDto> GetExternalTablesByDatabase(int databseId);
       BuildTableDto CheckTableDecriptionOfExcelSheet(int buildId, string tableDescription);
    }
}
