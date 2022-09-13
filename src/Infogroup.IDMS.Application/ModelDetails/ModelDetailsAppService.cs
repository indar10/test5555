

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ModelDetails.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.ModelDetails
{
    [AbpAuthorize]
	public class ModelDetailsAppService : IDMSAppServiceBase, IModelDetailsAppService
    {
		 private readonly IRepository<ModelDetail> _modelDetailRepository;
		 

		  public ModelDetailsAppService(IRepository<ModelDetail> modelDetailRepository ) 
		  {
			_modelDetailRepository = modelDetailRepository;
			
		  }

		 public async Task<PagedResultDto<GetModelDetailForViewDto>> GetAll(GetAllModelDetailsInput input)
         {
			
			var filteredModelDetails = _modelDetailRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cSQL_Score.Contains(input.Filter) || e.cSQL_Deciles.Contains(input.Filter) || e.cSQL_Preselect.Contains(input.Filter) || e.cSAS_ScoreFileName.Contains(input.Filter) || e.cSAS_ScoreRealFileName.Contains(input.Filter) || e.cSampleScoredByLast.Contains(input.Filter) || e.cFinalScoredByLast.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter) || e.cCompleteSQL.Contains(input.Filter));

			var pagedAndFilteredModelDetails = filteredModelDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var modelDetails = from o in pagedAndFilteredModelDetails
                         select new GetModelDetailForViewDto() {
							ModelDetail = new ModelDetailDto
							{
                                //Id = o.Id
							}
						};

            var totalCount = await filteredModelDetails.CountAsync();

            return new PagedResultDto<GetModelDetailForViewDto>(
                totalCount,
                await modelDetails.ToListAsync()
            );
         }
		 
		 public async Task<GetModelDetailForEditOutput> GetModelDetailForEdit(EntityDto input)
         {
            var modelDetail = await _modelDetailRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetModelDetailForEditOutput {ModelDetail = ObjectMapper.Map<CreateOrEditModelDetailDto>(modelDetail)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditModelDetailDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditModelDetailDto input)
         {
            var modelDetail = ObjectMapper.Map<ModelDetail>(input);

			

            await _modelDetailRepository.InsertAsync(modelDetail);
         }

		 protected virtual async Task Update(CreateOrEditModelDetailDto input)
         {
            var modelDetail = await _modelDetailRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, modelDetail);
         }

		 public async Task Delete(EntityDto input)
         {
            await _modelDetailRepository.DeleteAsync(input.Id);
         } 
    }
}