using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.IDMSTasks.Dtos;
using Infogroup.IDMS.Dto;
using System.Collections.Generic;

namespace Infogroup.IDMS.IDMSTasks
{
    public interface IIDMSTasksAppService : IApplicationService 
    {
        Task<PagedResultDto<GetIDMSTaskForViewDto>> GetAllTask(GetAllIDMSTasksInput input);

    }
}