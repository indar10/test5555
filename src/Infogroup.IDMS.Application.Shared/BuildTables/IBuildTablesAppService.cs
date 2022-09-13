using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.BuildTables.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.BuildTables
{
    public interface IBuildTablesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBuildTableForViewDto>> GetAll(GetAllBuildTablesInput input);

        Task<GetBuildTableForViewDto> GetBuildTableForView(int id);

		Task<GetBuildTableForEditOutput> GetBuildTableForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditBuildTableDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<BuildTableBuildLookupTableDto>> GetAllBuildForLookupTable(GetAllForLookupTableInput input);
		
    }
}