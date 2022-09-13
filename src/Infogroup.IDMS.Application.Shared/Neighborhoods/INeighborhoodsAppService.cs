using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Neighborhoods.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.Neighborhoods
{
    public interface INeighborhoodsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetNeighborhoodForViewDto>> GetAll(GetAllNeighborhoodsInput input);

		Task<GetNeighborhoodForEditOutput> GetNeighborhoodForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditNeighborhoodDto input);

		Task Delete(EntityDto input);

		
    }
}