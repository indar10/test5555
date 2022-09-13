using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.SICFranchiseCodes.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.SICFranchiseCodes
{
    public interface ISICFranchiseCodesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSICFranchiseCodeForViewDto>> GetAll(GetAllSICFranchiseCodesInput input);

		Task<GetSICFranchiseCodeForEditOutput> GetSICFranchiseCodeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSICFranchiseCodeDto input);

		Task Delete(EntityDto input);

		
    }
}