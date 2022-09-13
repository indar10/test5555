using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Builds.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Builds
{
    public interface IBuildsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBuildForViewDto>> GetAll(GetAllBuildsInput input);

        Task<GetBuildForViewDto> GetBuildForView(int id);

		Task<GetBuildForEditOutput> GetBuildForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditBuildDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<BuildDatabaseLookupTableDto>> GetAllDatabaseForLookupTable(GetAllForLookupTableInput input);

        int GetLatestBuildFromDatabaseID(int databaseID);

        List<DropdownOutputDto> GetChildAndExternalTablesByBuild(int buildId, int databaseId);
    }
}