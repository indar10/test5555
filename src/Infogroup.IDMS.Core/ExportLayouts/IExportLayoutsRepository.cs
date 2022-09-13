using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.ExportLayouts
{
    public interface IExportLayoutsRepository : IRepository<ExportLayout, int>
    {
        Task<PagedResultDto<GetExportLayoutForViewDto>> GetAllExportLayoutsList(Tuple<string, string, List<SqlParameter>> query);
       
        List<DropdownOutputDto> GetGroupDataByDatabaseAndUserID(string Query, List<SqlParameter> sqlParameters);
        List<ExportLayoutFieldsDto> GetExportLayoutFields(int iExportLOID);
        void CopyExportLayout(GetExportLayoutForViewDto record);
        List<GetExportLayoutSelectedFieldsDto> GetExportLayoutSelectedFields(int iExportLOID, int buildId);
        List<GetExportLayoutAddFieldsDto> GetExportLayoutAddFields(int tableId, int exportLayoutId);
        void ReorderExportLayoutOrderId(int id, int orderId, string modifiedBy);
        int CopyAllExportLayout(CopyAllExportLayoutDto input, string intiatedBy, string layoutIds);
        PagedResultDto<GetCopyAllExportLayoutForViewDto> GetExportLayoutForCopy(Tuple<string, string, List<SqlParameter>> query);
    }
}
