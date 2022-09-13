using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.CampaignAttachments.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.CampaignAttachments
{
    public interface ICampaignAttachmentsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCampaignAttachmentForViewDto>> GetAll(GetAllCampaignAttachmentsInput input);

		Task<GetCampaignAttachmentForEditOutput> GetCampaignAttachmentForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCampaignAttachmentDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<CampaignAttachmentCampaignLookupTableDto>> GetAllCampaignForLookupTable(GetAllForLookupTableInput input);
		
    }
}