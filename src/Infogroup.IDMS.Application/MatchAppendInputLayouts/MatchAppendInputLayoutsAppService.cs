using Infogroup.IDMS.MatchAppends;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.MatchAppendInputLayouts.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.MatchAppendInputLayouts
{
	[AbpAuthorize]
    public class MatchAppendInputLayoutsAppService : IDMSAppServiceBase, IMatchAppendInputLayoutsAppService
    {
		 private readonly IRepository<MatchAppendInputLayout> _matchAppendInputLayoutRepository;
		 private readonly IRepository<MatchAppend,int> _lookup_matchAppendRepository;
		 

		  public MatchAppendInputLayoutsAppService(IRepository<MatchAppendInputLayout> matchAppendInputLayoutRepository , IRepository<MatchAppend, int> lookup_matchAppendRepository) 
		  {
			_matchAppendInputLayoutRepository = matchAppendInputLayoutRepository;
			_lookup_matchAppendRepository = lookup_matchAppendRepository;
		
		  }

		 public async Task<PagedResultDto<GetMatchAppendInputLayoutForViewDto>> GetAll(GetAllMatchAppendInputLayoutsInput input)
         {
			
			var filteredMatchAppendInputLayouts = _matchAppendInputLayoutRepository.GetAll()
						.Include( e => e.MatchAppendFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cFieldName.Contains(input.Filter) || e.cMCMapping.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.MatchAppendcClientNameFilter), e => e.MatchAppendFk != null && e.MatchAppendFk.cClientName == input.MatchAppendcClientNameFilter);

			var pagedAndFilteredMatchAppendInputLayouts = filteredMatchAppendInputLayouts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var matchAppendInputLayouts = from o in pagedAndFilteredMatchAppendInputLayouts
                         join o1 in _lookup_matchAppendRepository.GetAll() on o.MatchAppendId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetMatchAppendInputLayoutForViewDto() {
							MatchAppendInputLayout = new MatchAppendInputLayoutDto
							{
                                Id = o.Id
							},
                         	MatchAppendcClientName = s1 == null ? "" : s1.cClientName.ToString()
						};

            var totalCount = await filteredMatchAppendInputLayouts.CountAsync();

            return new PagedResultDto<GetMatchAppendInputLayoutForViewDto>(
                totalCount,
                await matchAppendInputLayouts.ToListAsync()
            );
         }
		 
		 [AbpAuthorize]
		 public async Task<GetMatchAppendInputLayoutForEditOutput> GetMatchAppendInputLayoutForEdit(EntityDto input)
         {
            var matchAppendInputLayout = await _matchAppendInputLayoutRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetMatchAppendInputLayoutForEditOutput {MatchAppendInputLayout = ObjectMapper.Map<CreateOrEditMatchAppendInputLayoutDto>(matchAppendInputLayout)};


            var _lookupMatchAppend = await _lookup_matchAppendRepository.FirstOrDefaultAsync((int)output.MatchAppendInputLayout.MatchAppendId);
            output.MatchAppendcClientName = _lookupMatchAppend.cClientName.ToString();
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditMatchAppendInputLayoutDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize]
		 protected virtual async Task Create(CreateOrEditMatchAppendInputLayoutDto input)
         {
            var matchAppendInputLayout = ObjectMapper.Map<MatchAppendInputLayout>(input);

			

            await _matchAppendInputLayoutRepository.InsertAsync(matchAppendInputLayout);
         }

		 [AbpAuthorize]
		 protected virtual async Task Update(CreateOrEditMatchAppendInputLayoutDto input)
         {
            var matchAppendInputLayout = await _matchAppendInputLayoutRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, matchAppendInputLayout);
         }

		 [AbpAuthorize]
         public async Task Delete(EntityDto input)
         {
            await _matchAppendInputLayoutRepository.DeleteAsync(input.Id);
         } 

		[AbpAuthorize]
         public async Task<PagedResultDto<MatchAppendInputLayoutMatchAppendLookupTableDto>> GetAllMatchAppendForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_matchAppendRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cClientName != null && e.cClientName.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var matchAppendList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<MatchAppendInputLayoutMatchAppendLookupTableDto>();
			foreach(var matchAppend in matchAppendList){
				lookupTableDtoList.Add(new MatchAppendInputLayoutMatchAppendLookupTableDto
				{
					Id = matchAppend.Id,
					DisplayName = matchAppend.cClientName?.ToString()
				});
			}

            return new PagedResultDto<MatchAppendInputLayoutMatchAppendLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}