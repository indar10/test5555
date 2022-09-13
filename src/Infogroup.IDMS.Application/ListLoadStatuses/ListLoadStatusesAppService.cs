

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ListLoadStatuses.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.ListLoadStatuses
{
	[AbpAuthorize]
    public class ListLoadStatusesAppService : IDMSAppServiceBase, IListLoadStatusesAppService
    {
		 private readonly IRepository<ListLoadStatus> _listLoadStatusRepository;
		 

		  public ListLoadStatusesAppService(IRepository<ListLoadStatus> listLoadStatusRepository ) 
		  {
			_listLoadStatusRepository = listLoadStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetListLoadStatusForViewDto>> GetAll(GetAllListLoadStatusesInput input)
         {
			
			var filteredListLoadStatuses = _listLoadStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.LK_LoadStatus.Contains(input.Filter) || e.cCalculation.Contains(input.Filter) || e.cNotes.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter));

			var pagedAndFilteredListLoadStatuses = filteredListLoadStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var listLoadStatuses = from o in pagedAndFilteredListLoadStatuses
                         select new GetListLoadStatusForViewDto() {
							ListLoadStatus = new ListLoadStatusDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredListLoadStatuses.CountAsync();

            return new PagedResultDto<GetListLoadStatusForViewDto>(
                totalCount,
                await listLoadStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetListLoadStatusForEditOutput> GetListLoadStatusForEdit(EntityDto input)
         {
            var listLoadStatus = await _listLoadStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetListLoadStatusForEditOutput {ListLoadStatus = ObjectMapper.Map<CreateOrEditListLoadStatusDto>(listLoadStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditListLoadStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditListLoadStatusDto input)
         {
            var listLoadStatus = ObjectMapper.Map<ListLoadStatus>(input);

			

            await _listLoadStatusRepository.InsertAsync(listLoadStatus);
         }

		 protected virtual async Task Update(CreateOrEditListLoadStatusDto input)
         {
            var listLoadStatus = await _listLoadStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, listLoadStatus);
         }

		 public async Task Delete(EntityDto input)
         {
            await _listLoadStatusRepository.DeleteAsync(input.Id);
         } 
    }
}