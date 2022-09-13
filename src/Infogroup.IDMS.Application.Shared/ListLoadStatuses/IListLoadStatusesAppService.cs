using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ListLoadStatuses.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.ListLoadStatuses
{
    public interface IListLoadStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetListLoadStatusForViewDto>> GetAll(GetAllListLoadStatusesInput input);

		Task<GetListLoadStatusForEditOutput> GetListLoadStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditListLoadStatusDto input);

		Task Delete(EntityDto input);

		
    }
}