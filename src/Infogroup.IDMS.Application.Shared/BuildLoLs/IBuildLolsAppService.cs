using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.BuildLoLs.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.BuildLoLs
{
    public interface IBuildLolsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBuildLolForViewDto>> GetAll(GetAllBuildLolsInput input);

        Task<GetBuildLolForViewDto> GetBuildLolForView(int id);

		Task<GetBuildLolForEditOutput> GetBuildLolForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditBuildLolDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetBuildLolsToExcel(GetAllBuildLolsForExcelInput input);

		
		Task<PagedResultDto<BuildLolBuildLookupTableDto>> GetAllBuildForLookupTable(GetAllForLookupTableInput input);
		
    }
}