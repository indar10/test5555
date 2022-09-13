

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.MatchAppendStatuses.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.MatchAppendStatuses
{
	[AbpAuthorize]
    public class MatchAppendStatusesAppService : IDMSAppServiceBase, IMatchAppendStatusesAppService
    {
		 private readonly IRepository<MatchAppendStatus> _matchAppendStatusRepository;
		 

		  public MatchAppendStatusesAppService(IRepository<MatchAppendStatus> matchAppendStatusRepository ) 
		  {
			_matchAppendStatusRepository = matchAppendStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetMatchAppendStatusForViewDto>> GetAll(GetAllMatchAppendStatusesInput input)
         {
			
			var filteredMatchAppendStatuses = _matchAppendStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter));

			var pagedAndFilteredMatchAppendStatuses = filteredMatchAppendStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var matchAppendStatuses = from o in pagedAndFilteredMatchAppendStatuses
                         select new GetMatchAppendStatusForViewDto() {
							MatchAppendStatus = new MatchAppendStatusDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredMatchAppendStatuses.CountAsync();

            return new PagedResultDto<GetMatchAppendStatusForViewDto>(
                totalCount,
                await matchAppendStatuses.ToListAsync()
            );
         }
		 
		 [AbpAuthorize]
		 public async Task<GetMatchAppendStatusForEditOutput> GetMatchAppendStatusForEdit(EntityDto input)
         {
            var matchAppendStatus = await _matchAppendStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetMatchAppendStatusForEditOutput {MatchAppendStatus = ObjectMapper.Map<CreateOrEditMatchAppendStatusDto>(matchAppendStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditMatchAppendStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize]
		 protected virtual async Task Create(CreateOrEditMatchAppendStatusDto input)
         {
            var matchAppendStatus = ObjectMapper.Map<MatchAppendStatus>(input);

			

            await _matchAppendStatusRepository.InsertAsync(matchAppendStatus);
         }

		 [AbpAuthorize]
		 protected virtual async Task Update(CreateOrEditMatchAppendStatusDto input)
         {
            var matchAppendStatus = await _matchAppendStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, matchAppendStatus);
         }

		 [AbpAuthorize]
         public async Task Delete(EntityDto input)
         {
            await _matchAppendStatusRepository.DeleteAsync(input.Id);
         } 
    }
}