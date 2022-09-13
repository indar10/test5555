using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.ModelDetails.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.ModelDetails
{
    public interface IModelDetailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetModelDetailForViewDto>> GetAll(GetAllModelDetailsInput input);

		Task<GetModelDetailForEditOutput> GetModelDetailForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditModelDetailDto input);

		Task Delete(EntityDto input);

		
    }
}