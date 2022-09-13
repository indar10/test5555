using Infogroup.IDMS.Databases;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.AutoSuppresses.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.AutoSuppresses
{
	[AbpAuthorize(AppPermissions.Pages_AutoSuppresses)]
    public class AutoSuppressesAppService : IDMSAppServiceBase, IAutoSuppressesAppService
    {
		 private readonly IRepository<AutoSuppress, Guid> _autoSuppressRepository;
		 private readonly IRepository<Database,int> _lookup_databaseRepository;
		 

		  public AutoSuppressesAppService(IRepository<AutoSuppress, Guid> autoSuppressRepository , IRepository<Database, int> lookup_databaseRepository) 
		  {
			_autoSuppressRepository = autoSuppressRepository;
			_lookup_databaseRepository = lookup_databaseRepository;
		
		  }

		 public async Task<PagedResultDto<GetAutoSuppressForViewDto>> GetAll(GetAllAutoSuppressesInput input)
         {
			
			var filteredAutoSuppresses = _autoSuppressRepository.GetAll()
						.Include( e => e.DatabaseFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cQuestionFieldName.Contains(input.Filter) || e.cQuestionDescription.Contains(input.Filter) || e.cJoinOperator.Contains(input.Filter) || e.cGrouping.Contains(input.Filter) || e.cValues.Contains(input.Filter) || e.cValueMode.Contains(input.Filter) || e.cDescriptions.Contains(input.Filter) || e.cValueOperator.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DatabasecDatabaseNameFilter), e => e.DatabaseFk != null && e.DatabaseFk.cDatabaseName.ToLower() == input.DatabasecDatabaseNameFilter.ToLower().Trim());

			var pagedAndFilteredAutoSuppresses = filteredAutoSuppresses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var autoSuppresses = from o in pagedAndFilteredAutoSuppresses
                         join o1 in _lookup_databaseRepository.GetAll() on o.DatabaseId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetAutoSuppressForViewDto() {
							AutoSuppress = new AutoSuppressDto
							{
                                Id = o.Id
							},
                         	DatabasecDatabaseName = s1 == null ? "" : s1.cDatabaseName.ToString()
						};

            var totalCount = await filteredAutoSuppresses.CountAsync();

            return new PagedResultDto<GetAutoSuppressForViewDto>(
                totalCount,
                await autoSuppresses.ToListAsync()
            );
         }
		 
		 public async Task<GetAutoSuppressForViewDto> GetAutoSuppressForView(Guid id)
         {
            var autoSuppress = await _autoSuppressRepository.GetAsync(id);

            var output = new GetAutoSuppressForViewDto { AutoSuppress = ObjectMapper.Map<AutoSuppressDto>(autoSuppress) };

		    if (output.AutoSuppress.DatabaseId != null)
            {
                var _lookupDatabase = await _lookup_databaseRepository.FirstOrDefaultAsync((int)output.AutoSuppress.DatabaseId);
                output.DatabasecDatabaseName = _lookupDatabase.cDatabaseName.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_AutoSuppresses_Edit)]
		 public async Task<GetAutoSuppressForEditOutput> GetAutoSuppressForEdit(EntityDto<Guid> input)
         {
            var autoSuppress = await _autoSuppressRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAutoSuppressForEditOutput {AutoSuppress = ObjectMapper.Map<CreateOrEditAutoSuppressDto>(autoSuppress)};

		    if (output.AutoSuppress.DatabaseId != null)
            {
                var _lookupDatabase = await _lookup_databaseRepository.FirstOrDefaultAsync((int)output.AutoSuppress.DatabaseId);
                output.DatabasecDatabaseName = _lookupDatabase.cDatabaseName.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAutoSuppressDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_AutoSuppresses_Create)]
		 protected virtual async Task Create(CreateOrEditAutoSuppressDto input)
         {
            var autoSuppress = ObjectMapper.Map<AutoSuppress>(input);

			

            await _autoSuppressRepository.InsertAsync(autoSuppress);
         }

		 [AbpAuthorize(AppPermissions.Pages_AutoSuppresses_Edit)]
		 protected virtual async Task Update(CreateOrEditAutoSuppressDto input)
         {
            var autoSuppress = await _autoSuppressRepository.FirstOrDefaultAsync((Guid)input.Id);
             ObjectMapper.Map(input, autoSuppress);
         }

		 [AbpAuthorize(AppPermissions.Pages_AutoSuppresses_Delete)]
         public async Task Delete(EntityDto<Guid> input)
         {
            await _autoSuppressRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize(AppPermissions.Pages_AutoSuppresses)]
         public async Task<PagedResultDto<AutoSuppressDatabaseLookupTableDto>> GetAllDatabaseForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_databaseRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cDatabaseName.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var databaseList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AutoSuppressDatabaseLookupTableDto>();
			foreach(var database in databaseList){
				lookupTableDtoList.Add(new AutoSuppressDatabaseLookupTableDto
				{
					Id = database.Id,
					DisplayName = database.cDatabaseName?.ToString()
				});
			}

            return new PagedResultDto<AutoSuppressDatabaseLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}