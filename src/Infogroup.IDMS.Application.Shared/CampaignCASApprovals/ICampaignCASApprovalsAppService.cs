using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.CampaignCASApprovals.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.CampaignCASApprovals
{
    public interface ICampaignCASApprovalsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCampaignCASApprovalForViewDto>> GetAll(GetAllCampaignCASApprovalsInput input);

		Task<GetCampaignCASApprovalForEditOutput> GetCampaignCASApprovalForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCampaignCASApprovalDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<CampaignCASApprovalCampaignLookupTableDto>> GetAllCampaignForLookupTable(GetAllForLookupTableInput input);
		
    }
}