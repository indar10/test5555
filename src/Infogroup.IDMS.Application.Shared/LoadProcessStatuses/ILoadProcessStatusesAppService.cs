using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.LoadProcessStatuses.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.LoadProcessStatuses
{
    public interface ILoadProcessStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetLoadProcessStatusForViewDto>> GetAll(GetAllLoadProcessStatusesInput input);

		Task<GetLoadProcessStatusForEditOutput> GetLoadProcessStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditLoadProcessStatusDto input);

		Task Delete(EntityDto input);

		
    }
}