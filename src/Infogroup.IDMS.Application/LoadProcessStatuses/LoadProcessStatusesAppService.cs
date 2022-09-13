

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.LoadProcessStatuses.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.LoadProcessStatuses
{
	[AbpAuthorize]
    public class LoadProcessStatusesAppService : IDMSAppServiceBase, ILoadProcessStatusesAppService
    {
		 private readonly IRepository<LoadProcessStatus> _loadProcessStatusRepository;
		 

		  public LoadProcessStatusesAppService(IRepository<LoadProcessStatus> loadProcessStatusRepository ) 
		  {
			_loadProcessStatusRepository = loadProcessStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetLoadProcessStatusForViewDto>> GetAll(GetAllLoadProcessStatusesInput input)
         {
			
			var filteredLoadProcessStatuses = _loadProcessStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter));

			var pagedAndFilteredLoadProcessStatuses = filteredLoadProcessStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var loadProcessStatuses = from o in pagedAndFilteredLoadProcessStatuses
                         select new GetLoadProcessStatusForViewDto() {
							LoadProcessStatus = new LoadProcessStatusDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredLoadProcessStatuses.CountAsync();

            return new PagedResultDto<GetLoadProcessStatusForViewDto>(
                totalCount,
                await loadProcessStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetLoadProcessStatusForEditOutput> GetLoadProcessStatusForEdit(EntityDto input)
         {
            var loadProcessStatus = await _loadProcessStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetLoadProcessStatusForEditOutput {LoadProcessStatus = ObjectMapper.Map<CreateOrEditLoadProcessStatusDto>(loadProcessStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditLoadProcessStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditLoadProcessStatusDto input)
         {
            var loadProcessStatus = ObjectMapper.Map<LoadProcessStatus>(input);

			

            await _loadProcessStatusRepository.InsertAsync(loadProcessStatus);
         }

		 protected virtual async Task Update(CreateOrEditLoadProcessStatusDto input)
         {
            var loadProcessStatus = await _loadProcessStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, loadProcessStatus);
         }

		 public async Task Delete(EntityDto input)
         {
            await _loadProcessStatusRepository.DeleteAsync(input.Id);
         } 
    }
}