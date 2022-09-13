using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.CampaignExportLayouts.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.CampaignExportLayouts
{
    public interface ICampaignExportLayoutsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCampaignExportLayoutForViewDto>> GetAll(GetAllCampaignExportLayoutsInput input);

		Task<GetCampaignExportLayoutForEditOutput> GetCampaignExportLayoutForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCampaignExportLayoutDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<CampaignExportLayoutCampaignLookupTableDto>> GetAllCampaignForLookupTable(GetAllForLookupTableInput input);
		
    }
}