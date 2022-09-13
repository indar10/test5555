using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Infogroup.IDMS.Segments.Dtos;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.ExportLayouts
{
    public interface IExportLayoutsAppService : IApplicationService
    {

        List<DropdownOutputDto> GetOutputCaseDropDownValues();
        List<DropdownOutputDto> GetTableDescriptionDropDownValues(int campaignId, int maintainanceBuildId, bool isCampaignExport, int databaseId);
        void ReorderExportLayoutOrderId(int id, int orderId, int campaignId, bool isCampaign);
        PagedResultDto<GetCopyAllExportLayoutForViewDto> GetAllExportLayout(GetAllExportLayoutForCopyDto input);
        void CopyAllExportLayout(CopyAllExportLayoutDto input);
    }
}