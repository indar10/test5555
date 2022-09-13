using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.AccessObjects.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.AccessObjects
{
    public interface IAccessObjectsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAccessObjectForViewDto>> GetAll(GetAllAccessObjectsInput input);

		Task<GetAccessObjectForEditOutput> GetAccessObjectForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAccessObjectDto input);

		Task Delete(EntityDto input);

		
    }
}