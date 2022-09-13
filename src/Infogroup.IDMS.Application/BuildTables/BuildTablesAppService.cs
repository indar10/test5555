using Infogroup.IDMS.Builds;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.BuildTables.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.BuildTables
{
	//[AbpAuthorize(AppPermissions.Pages_BuildTables)]
    public class BuildTablesAppService : IDMSAppServiceBase, IBuildTablesAppService
    {
		 private readonly IRepository<BuildTable> _buildTableRepository;
		 private readonly IRepository<Build,int> _buildRepository;
		 

		  public BuildTablesAppService(IRepository<BuildTable> buildTableRepository , IRepository<Build, int> buildRepository) 
		  {
			_buildTableRepository = buildTableRepository;
			_buildRepository = buildRepository;
		
		  }

		 public async Task<PagedResultDto<GetBuildTableForViewDto>> GetAll(GetAllBuildTablesInput input)
         {
			
			var filteredBuildTables = _buildTableRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cTableName.Contains(input.Filter) || e.LK_TableType.Contains(input.Filter) || e.LK_JoinType.Contains(input.Filter) || e.cJoinOn.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter) || e.ctabledescription.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.cTableNameFilter),  e => e.cTableName.ToLower() == input.cTableNameFilter.ToLower().Trim());


			var query = (from o in filteredBuildTables
                         join o1 in _buildRepository.GetAll() on o.BuildId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetBuildTableForViewDto() {
							BuildTable = ObjectMapper.Map<BuildTableDto>(o),
                         	BuildcBuild = s1 == null ? "" : s1.cBuild.ToString()
						})
						.WhereIf(!string.IsNullOrWhiteSpace(input.BuildcBuildFilter), e => e.BuildcBuild.ToLower() == input.BuildcBuildFilter.ToLower().Trim());

            var totalCount = await query.CountAsync();

            var buildTables = await query
                .OrderBy(input.Sorting ?? "buildTable.id asc")
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<GetBuildTableForViewDto>(
                totalCount,
                buildTables
            );
         }
		 
		 public async Task<GetBuildTableForViewDto> GetBuildTableForView(int id)
         {
            var buildTable = await _buildTableRepository.GetAsync(id);

            var output = new GetBuildTableForViewDto { BuildTable = ObjectMapper.Map<BuildTableDto>(buildTable) };


            var build = await _buildRepository.FirstOrDefaultAsync((int)output.BuildTable.BuildId);
            output.BuildcBuild = build.cBuild.ToString();
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_BuildTables_Edit)]
		 public async Task<GetBuildTableForEditOutput> GetBuildTableForEdit(EntityDto input)
         {
            var buildTable = await _buildTableRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetBuildTableForEditOutput {BuildTable = ObjectMapper.Map<CreateOrEditBuildTableDto>(buildTable)};


            var build = await _buildRepository.FirstOrDefaultAsync((int)output.BuildTable.BuildId);
            output.BuildcBuild = build.cBuild.ToString();
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditBuildTableDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_BuildTables_Create)]
		 private async Task Create(CreateOrEditBuildTableDto input)
         {
            var buildTable = ObjectMapper.Map<BuildTable>(input);

			

            await _buildTableRepository.InsertAsync(buildTable);
         }

		 [AbpAuthorize(AppPermissions.Pages_BuildTables_Edit)]
		 private async Task Update(CreateOrEditBuildTableDto input)
         {
            var buildTable = await _buildTableRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, buildTable);
         }

		 [AbpAuthorize(AppPermissions.Pages_BuildTables_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _buildTableRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_BuildTables)]
         public async Task<PagedResultDto<BuildTableBuildLookupTableDto>> GetAllBuildForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _buildRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cBuild.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var buildList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BuildTableBuildLookupTableDto>();
			foreach(var build in buildList){
				lookupTableDtoList.Add(new BuildTableBuildLookupTableDto
				{
					Id = build.Id,
					DisplayName = build.cBuild?.ToString()
				});
			}

            return new PagedResultDto<BuildTableBuildLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}