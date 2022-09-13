

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SICFranchiseCodes.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.SICFranchiseCodes
{
	[AbpAuthorize]
    public class SICFranchiseCodesAppService : IDMSAppServiceBase, ISICFranchiseCodesAppService
    {
		 private readonly IRepository<SICFranchiseCode> _sicFranchiseCodeRepository;
		 

		  public SICFranchiseCodesAppService(IRepository<SICFranchiseCode> sicFranchiseCodeRepository ) 
		  {
			_sicFranchiseCodeRepository = sicFranchiseCodeRepository;
			
		  }

		 public async Task<PagedResultDto<GetSICFranchiseCodeForViewDto>> GetAll(GetAllSICFranchiseCodesInput input)
         {
			
			var filteredSICFranchiseCodes = _sicFranchiseCodeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cSICCode.Contains(input.Filter) || e.cSICDescription.Contains(input.Filter) || e.cFranchiseCode.Contains(input.Filter) || e.cConvertedFranchise.Contains(input.Filter) || e.cFranchiseName.Contains(input.Filter) || e.cFranchiseType.Contains(input.Filter) || e.cCanadianFlag.Contains(input.Filter) || e.cOldFranchiseCode.Contains(input.Filter) || e.cTransactionDate.Contains(input.Filter) || e.CRLF.Contains(input.Filter));

			var pagedAndFilteredSICFranchiseCodes = filteredSICFranchiseCodes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var sicFranchiseCodes = from o in pagedAndFilteredSICFranchiseCodes
                         select new GetSICFranchiseCodeForViewDto() {
							SICFranchiseCode = new SICFranchiseCodeDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredSICFranchiseCodes.CountAsync();

            return new PagedResultDto<GetSICFranchiseCodeForViewDto>(
                totalCount,
                await sicFranchiseCodes.ToListAsync()
            );
         }
		 
		 public async Task<GetSICFranchiseCodeForEditOutput> GetSICFranchiseCodeForEdit(EntityDto input)
         {
            var sicFranchiseCode = await _sicFranchiseCodeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetSICFranchiseCodeForEditOutput {SICFranchiseCode = ObjectMapper.Map<CreateOrEditSICFranchiseCodeDto>(sicFranchiseCode)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditSICFranchiseCodeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditSICFranchiseCodeDto input)
         {
            var sicFranchiseCode = ObjectMapper.Map<SICFranchiseCode>(input);

			

            await _sicFranchiseCodeRepository.InsertAsync(sicFranchiseCode);
         }

		 protected virtual async Task Update(CreateOrEditSICFranchiseCodeDto input)
         {
            var sicFranchiseCode = await _sicFranchiseCodeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, sicFranchiseCode);
         }

		 public async Task Delete(EntityDto input)
         {
            await _sicFranchiseCodeRepository.DeleteAsync(input.Id);
         } 
    }
}