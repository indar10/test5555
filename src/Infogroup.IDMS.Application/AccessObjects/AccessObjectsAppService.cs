

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.AccessObjects.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.AccessObjects
{
    [AbpAuthorize]
	public class AccessObjectsAppService : IDMSAppServiceBase, IAccessObjectsAppService
    {
		 private readonly IRepository<AccessObject> _accessObjectRepository;
		 

		  public AccessObjectsAppService(IRepository<AccessObject> accessObjectRepository ) 
		  {
			_accessObjectRepository = accessObjectRepository;
			
		  }

		 public async Task<PagedResultDto<GetAccessObjectForViewDto>> GetAll(GetAllAccessObjectsInput input)
         {
			
			var filteredAccessObjects = _accessObjectRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCode.Contains(input.Filter) || e.cDescription.Contains(input.Filter) || e.cPath.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter));

			var pagedAndFilteredAccessObjects = filteredAccessObjects
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var accessObjects = from o in pagedAndFilteredAccessObjects
                         select new GetAccessObjectForViewDto() {
							AccessObject = new AccessObjectDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredAccessObjects.CountAsync();

            return new PagedResultDto<GetAccessObjectForViewDto>(
                totalCount,
                await accessObjects.ToListAsync()
            );
         }
		 
		 public async Task<GetAccessObjectForEditOutput> GetAccessObjectForEdit(EntityDto input)
         {
            var accessObject = await _accessObjectRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAccessObjectForEditOutput {AccessObject = ObjectMapper.Map<CreateOrEditAccessObjectDto>(accessObject)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAccessObjectDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditAccessObjectDto input)
         {
            var accessObject = ObjectMapper.Map<AccessObject>(input);

			

            await _accessObjectRepository.InsertAsync(accessObject);
         }

		 protected virtual async Task Update(CreateOrEditAccessObjectDto input)
         {
            var accessObject = await _accessObjectRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, accessObject);
         }

		 public async Task Delete(EntityDto input)
         {
            await _accessObjectRepository.DeleteAsync(input.Id);
         } 
    }
}