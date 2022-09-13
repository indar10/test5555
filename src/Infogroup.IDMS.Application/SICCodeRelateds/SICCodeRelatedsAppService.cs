using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SICCodeRelateds.Dtos;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.SICCodeRelateds
{
	[AbpAuthorize]
    public class SICCodeRelatedsAppService : IDMSAppServiceBase, ISICCodeRelatedsAppService
    {
		 private readonly IRepository<SICCodeRelated> _sicCodeRelatedRepository;
		 

		  public SICCodeRelatedsAppService(IRepository<SICCodeRelated> sicCodeRelatedRepository ) 
		  {
			_sicCodeRelatedRepository = sicCodeRelatedRepository;
			
		  }

		 public async Task<PagedResultDto<GetSICCodeRelatedForViewDto>> GetAll(GetAllSICCodeRelatedsInput input)
         {
			
			var filteredSICCodeRelateds = _sicCodeRelatedRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cSICCode.Contains(input.Filter) || e.cRelatedSICCode.Contains(input.Filter) || e.cRelatedSICDescription.Contains(input.Filter) || e.cIndicator.Contains(input.Filter));

			var pagedAndFilteredSICCodeRelateds = filteredSICCodeRelateds
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var sicCodeRelateds = from o in pagedAndFilteredSICCodeRelateds
                         select new GetSICCodeRelatedForViewDto() {
							SICCodeRelated = new SICCodeRelatedDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredSICCodeRelateds.CountAsync();

            return new PagedResultDto<GetSICCodeRelatedForViewDto>(
                totalCount,
                await sicCodeRelateds.ToListAsync()
            );
         }
		 
		 public async Task<GetSICCodeRelatedForEditOutput> GetSICCodeRelatedForEdit(EntityDto input)
         {
            var sicCodeRelated = await _sicCodeRelatedRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetSICCodeRelatedForEditOutput {SICCodeRelated = ObjectMapper.Map<CreateOrEditSICCodeRelatedDto>(sicCodeRelated)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditSICCodeRelatedDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditSICCodeRelatedDto input)
         {
            var sicCodeRelated = ObjectMapper.Map<SICCodeRelated>(input);

			

            await _sicCodeRelatedRepository.InsertAsync(sicCodeRelated);
         }

		 protected virtual async Task Update(CreateOrEditSICCodeRelatedDto input)
         {
            var sicCodeRelated = await _sicCodeRelatedRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, sicCodeRelated);
         }

		 public async Task Delete(EntityDto input)
         {
            await _sicCodeRelatedRepository.DeleteAsync(input.Id);
         } 
    }
}