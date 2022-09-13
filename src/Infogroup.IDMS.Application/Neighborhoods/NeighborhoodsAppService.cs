

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Neighborhoods.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.Neighborhoods
{
	[AbpAuthorize]
    public class NeighborhoodsAppService : IDMSAppServiceBase, INeighborhoodsAppService
    {
		 private readonly IRepository<Neighborhood> _neighborhoodRepository;
		 

		  public NeighborhoodsAppService(IRepository<Neighborhood> neighborhoodRepository ) 
		  {
			_neighborhoodRepository = neighborhoodRepository;
			
		  }

		 public async Task<PagedResultDto<GetNeighborhoodForViewDto>> GetAll(GetAllNeighborhoodsInput input)
         {
			
			var filteredNeighborhoods = _neighborhoodRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cState.Contains(input.Filter) || e.cNeighborhood.Contains(input.Filter));

			var pagedAndFilteredNeighborhoods = filteredNeighborhoods
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var neighborhoods = from o in pagedAndFilteredNeighborhoods
                         select new GetNeighborhoodForViewDto() {
							Neighborhood = new NeighborhoodDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredNeighborhoods.CountAsync();

            return new PagedResultDto<GetNeighborhoodForViewDto>(
                totalCount,
                await neighborhoods.ToListAsync()
            );
         }
		 
		 [AbpAuthorize]
		 public async Task<GetNeighborhoodForEditOutput> GetNeighborhoodForEdit(EntityDto input)
         {
            var neighborhood = await _neighborhoodRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetNeighborhoodForEditOutput {Neighborhood = ObjectMapper.Map<CreateOrEditNeighborhoodDto>(neighborhood)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditNeighborhoodDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize]
		 protected virtual async Task Create(CreateOrEditNeighborhoodDto input)
         {
            var neighborhood = ObjectMapper.Map<Neighborhood>(input);

			

            await _neighborhoodRepository.InsertAsync(neighborhood);
         }

		 [AbpAuthorize]
		 protected virtual async Task Update(CreateOrEditNeighborhoodDto input)
         {
            var neighborhood = await _neighborhoodRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, neighborhood);
         }

		 [AbpAuthorize]
         public async Task Delete(EntityDto input)
         {
            await _neighborhoodRepository.DeleteAsync(input.Id);
         } 
    }
}