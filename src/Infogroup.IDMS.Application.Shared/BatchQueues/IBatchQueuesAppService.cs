using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.BatchQueues.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.BatchQueues
{
    public interface IBatchQueuesAppService : IApplicationService 
    {
        PagedResultDto<BatchQueueDto> GetAll(GetAllBatchQueuesInput input);
        CreateOrEditBatchQueueDto GetQueuesData(int queueId);
        void CreateOrEdit(int id);


    }
}