

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ModelQueues.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.ModelQueues
{
    [AbpAuthorize]
	public class ModelQueuesAppService : IDMSAppServiceBase, IModelQueuesAppService
    {
		 private readonly IRepository<ModelQueue> _modelQueueRepository;
		 

		  public ModelQueuesAppService(IRepository<ModelQueue> modelQueueRepository ) 
		  {
			_modelQueueRepository = modelQueueRepository;
			
		  }

		 public async Task<PagedResultDto<GetModelQueueForViewDto>> GetAll(GetAllModelQueuesInput input)
         {
			
			var filteredModelQueues = _modelQueueRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.LK_ModelStatus.Contains(input.Filter) || e.cNotes.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter));

			var pagedAndFilteredModelQueues = filteredModelQueues
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var modelQueues = from o in pagedAndFilteredModelQueues
                         select new GetModelQueueForViewDto() {
							ModelQueue = new ModelQueueDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredModelQueues.CountAsync();

            return new PagedResultDto<GetModelQueueForViewDto>(
                totalCount,
                await modelQueues.ToListAsync()
            );
         }
		 
		 public async Task<GetModelQueueForEditOutput> GetModelQueueForEdit(EntityDto input)
         {
            var modelQueue = await _modelQueueRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetModelQueueForEditOutput {ModelQueue = ObjectMapper.Map<CreateOrEditModelQueueDto>(modelQueue)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditModelQueueDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditModelQueueDto input)
         {
            var modelQueue = ObjectMapper.Map<ModelQueue>(input);

			

            await _modelQueueRepository.InsertAsync(modelQueue);
         }

		 protected virtual async Task Update(CreateOrEditModelQueueDto input)
         {
            var modelQueue = await _modelQueueRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, modelQueue);
         }

		 public async Task Delete(EntityDto input)
         {
            await _modelQueueRepository.DeleteAsync(input.Id);
         } 
    }
}