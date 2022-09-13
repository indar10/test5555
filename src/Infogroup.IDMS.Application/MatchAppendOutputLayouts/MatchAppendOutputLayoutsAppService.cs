

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.MatchAppendOutputLayouts.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.MatchAppendOutputLayouts
{
	[AbpAuthorize]
    public class MatchAppendOutputLayoutsAppService : IDMSAppServiceBase, IMatchAppendOutputLayoutsAppService
    {
		 private readonly IRepository<MatchAppendOutputLayout> _matchAppendOutputLayoutRepository;
		 

		  public MatchAppendOutputLayoutsAppService(IRepository<MatchAppendOutputLayout> matchAppendOutputLayoutRepository ) 
		  {
			_matchAppendOutputLayoutRepository = matchAppendOutputLayoutRepository;
			
		  }

		 public async Task<PagedResultDto<GetMatchAppendOutputLayoutForViewDto>> GetAll(GetAllMatchAppendOutputLayoutsInput input)
         {
			
			var filteredMatchAppendOutputLayouts = _matchAppendOutputLayoutRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cTableName.Contains(input.Filter) || e.cFieldName.Contains(input.Filter) || e.cOutputFieldName.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter));

			var pagedAndFilteredMatchAppendOutputLayouts = filteredMatchAppendOutputLayouts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var matchAppendOutputLayouts = from o in pagedAndFilteredMatchAppendOutputLayouts
                         select new GetMatchAppendOutputLayoutForViewDto() {
							MatchAppendOutputLayout = new MatchAppendOutputLayoutDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredMatchAppendOutputLayouts.CountAsync();

            return new PagedResultDto<GetMatchAppendOutputLayoutForViewDto>(
                totalCount,
                await matchAppendOutputLayouts.ToListAsync()
            );
         }
		 
		 [AbpAuthorize]
		 public async Task<GetMatchAppendOutputLayoutForEditOutput> GetMatchAppendOutputLayoutForEdit(EntityDto input)
         {
            var matchAppendOutputLayout = await _matchAppendOutputLayoutRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetMatchAppendOutputLayoutForEditOutput {MatchAppendOutputLayout = ObjectMapper.Map<CreateOrEditMatchAppendOutputLayoutDto>(matchAppendOutputLayout)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditMatchAppendOutputLayoutDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize]
		 protected virtual async Task Create(CreateOrEditMatchAppendOutputLayoutDto input)
         {
            var matchAppendOutputLayout = ObjectMapper.Map<MatchAppendOutputLayout>(input);

			

            await _matchAppendOutputLayoutRepository.InsertAsync(matchAppendOutputLayout);
         }

		 [AbpAuthorize]
		 protected virtual async Task Update(CreateOrEditMatchAppendOutputLayoutDto input)
         {
            var matchAppendOutputLayout = await _matchAppendOutputLayoutRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, matchAppendOutputLayout);
         }

		 [AbpAuthorize]
         public async Task Delete(EntityDto input)
         {
            await _matchAppendOutputLayoutRepository.DeleteAsync(input.Id);
         } 
    }
}