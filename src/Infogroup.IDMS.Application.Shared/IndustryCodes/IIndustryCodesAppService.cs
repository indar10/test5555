using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.IndustryCodes.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.IndustryCodes
{
    public interface IIndustryCodesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetIndustryCodeForViewDto>> GetAll(GetAllIndustryCodesInput input);

		Task<GetIndustryCodeForEditOutput> GetIndustryCodeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditIndustryCodeDto input);

		Task Delete(EntityDto input);

		
    }
}