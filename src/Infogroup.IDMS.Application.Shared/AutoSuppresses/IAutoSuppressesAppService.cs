using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.AutoSuppresses.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.AutoSuppresses
{
    public interface IAutoSuppressesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAutoSuppressForViewDto>> GetAll(GetAllAutoSuppressesInput input);

        Task<GetAutoSuppressForViewDto> GetAutoSuppressForView(Guid id);

		Task<GetAutoSuppressForEditOutput> GetAutoSuppressForEdit(EntityDto<Guid> input);

		Task CreateOrEdit(CreateOrEditAutoSuppressDto input);

		Task Delete(EntityDto<Guid> input);

		
		Task<PagedResultDto<AutoSuppressDatabaseLookupTableDto>> GetAllDatabaseForLookupTable(GetAllForLookupTableInput input);
		
    }
}