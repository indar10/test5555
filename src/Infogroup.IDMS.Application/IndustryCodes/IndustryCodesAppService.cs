

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.IndustryCodes.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.IndustryCodes
{
	[AbpAuthorize]
    public class IndustryCodesAppService : IDMSAppServiceBase, IIndustryCodesAppService
    {
		 private readonly IRepository<IndustryCode> _industryCodeRepository;
		 

		  public IndustryCodesAppService(IRepository<IndustryCode> industryCodeRepository ) 
		  {
			_industryCodeRepository = industryCodeRepository;
			
		  }

		 public async Task<PagedResultDto<GetIndustryCodeForViewDto>> GetAll(GetAllIndustryCodesInput input)
         {
			
			var filteredIndustryCodes = _industryCodeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cSICCode.Contains(input.Filter) || e.cIndustrySpecificCode.Contains(input.Filter) || e.cPositionIndicator.Contains(input.Filter) || e.cIndustrySpecificDescription.Contains(input.Filter) || e.cRangeFromValue.Contains(input.Filter) || e.cRangeToValue.Contains(input.Filter));

			var pagedAndFilteredIndustryCodes = filteredIndustryCodes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var industryCodes = from o in pagedAndFilteredIndustryCodes
                         select new GetIndustryCodeForViewDto() {
							IndustryCode = new IndustryCodeDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredIndustryCodes.CountAsync();

            return new PagedResultDto<GetIndustryCodeForViewDto>(
                totalCount,
                await industryCodes.ToListAsync()
            );
         }
		 
		 public async Task<GetIndustryCodeForEditOutput> GetIndustryCodeForEdit(EntityDto input)
         {
            var industryCode = await _industryCodeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetIndustryCodeForEditOutput {IndustryCode = ObjectMapper.Map<CreateOrEditIndustryCodeDto>(industryCode)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditIndustryCodeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditIndustryCodeDto input)
         {
            var industryCode = ObjectMapper.Map<IndustryCode>(input);

			

            await _industryCodeRepository.InsertAsync(industryCode);
         }

		 protected virtual async Task Update(CreateOrEditIndustryCodeDto input)
         {
            var industryCode = await _industryCodeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, industryCode);
         }

		 public async Task Delete(EntityDto input)
         {
            await _industryCodeRepository.DeleteAsync(input.Id);
         } 
    }
}