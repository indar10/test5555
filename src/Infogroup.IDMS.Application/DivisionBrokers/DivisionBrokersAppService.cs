

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.DivisionBrokers.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.DivisionBrokers
{
	[AbpAuthorize(AppPermissions.Pages_DivisionBrokers)]
    public class DivisionBrokersAppService : IDMSAppServiceBase, IDivisionBrokersAppService
    {
		 private readonly IRepository<DivisionBroker> _divisionBrokerRepository;
		 

		  public DivisionBrokersAppService(IRepository<DivisionBroker> divisionBrokerRepository ) 
		  {
			_divisionBrokerRepository = divisionBrokerRepository;
			
		  }

		 public async Task<PagedResultDto<GetDivisionBrokerForViewDto>> GetAll(GetAllDivisionBrokersInput input)
         {
			
			var filteredDivisionBrokers = _divisionBrokerRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCode.Contains(input.Filter) || e.cCompany.Contains(input.Filter) || e.cFirstName.Contains(input.Filter) || e.cLastName.Contains(input.Filter) || e.cAddr1.Contains(input.Filter) || e.cAddr2.Contains(input.Filter) || e.cAddr3.Contains(input.Filter) || e.cCity.Contains(input.Filter) || e.cState.Contains(input.Filter) || e.cZip.Contains(input.Filter) || e.cCountry.Contains(input.Filter) || e.cPhone.Contains(input.Filter) || e.cFax.Contains(input.Filter) || e.cEmail.Contains(input.Filter) || e.mNotes.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter));

			var pagedAndFilteredDivisionBrokers = filteredDivisionBrokers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var divisionBrokers = from o in pagedAndFilteredDivisionBrokers
                         select new GetDivisionBrokerForViewDto() {
							DivisionBroker = new DivisionBrokerDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredDivisionBrokers.CountAsync();

            return new PagedResultDto<GetDivisionBrokerForViewDto>(
                totalCount,
                await divisionBrokers.ToListAsync()
            );
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_DivisionBrokers_Edit)]
		 public async Task<GetDivisionBrokerForEditOutput> GetDivisionBrokerForEdit(EntityDto input)
         {
            var divisionBroker = await _divisionBrokerRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetDivisionBrokerForEditOutput {DivisionBroker = ObjectMapper.Map<CreateOrEditDivisionBrokerDto>(divisionBroker)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditDivisionBrokerDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_DivisionBrokers_Create)]
		 protected virtual async Task Create(CreateOrEditDivisionBrokerDto input)
         {
            var divisionBroker = ObjectMapper.Map<DivisionBroker>(input);

			

            await _divisionBrokerRepository.InsertAsync(divisionBroker);
         }

		 [AbpAuthorize(AppPermissions.Pages_DivisionBrokers_Edit)]
		 protected virtual async Task Update(CreateOrEditDivisionBrokerDto input)
         {
            var divisionBroker = await _divisionBrokerRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, divisionBroker);
         }

		 [AbpAuthorize(AppPermissions.Pages_DivisionBrokers_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _divisionBrokerRepository.DeleteAsync(input.Id);
         } 
    }
}