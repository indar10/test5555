using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ModelQueues.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.ModelQueues
{
    public interface IModelQueuesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetModelQueueForViewDto>> GetAll(GetAllModelQueuesInput input);

		Task<GetModelQueueForEditOutput> GetModelQueueForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditModelQueueDto input);

		Task Delete(EntityDto input);

		
    }
}