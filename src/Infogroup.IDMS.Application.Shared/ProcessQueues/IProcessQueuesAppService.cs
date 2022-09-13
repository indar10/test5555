using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ProcessQueues.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.ProcessQueues
{
    public interface IProcessQueuesAppService : IApplicationService 
    {
        PagedResultDto<ProcessQueueDto> GetAll(GetAllForLookupTableInput input);

        Task<GetProcessQueueForViewDto> GetProcessQueueForView(int id);	
		Task CreateOrEdit(CreateOrEditProcessQueueDto input);

		Task Delete(EntityDto input);

		

		
    }
}