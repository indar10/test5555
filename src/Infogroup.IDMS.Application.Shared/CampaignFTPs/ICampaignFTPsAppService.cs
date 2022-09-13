using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.CampaignFTPs.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.CampaignFTPs
{
    public interface ICampaignFTPsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCampaignFTPForViewDto>> GetAll(GetAllCampaignFTPsInput input);

		Task<GetCampaignFTPForEditOutput> GetCampaignFTPForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCampaignFTPDto input);

		Task Delete(EntityDto input);

		
    }
}