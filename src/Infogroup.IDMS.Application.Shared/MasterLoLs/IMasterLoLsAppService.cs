using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.MasterLoLs.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.MasterLoLs
{
    public interface IMasterLoLsAppService : IApplicationService 
    {
        PagedResultDto<MasterLoLForViewDto> GetAll(GetAllMasterLoLsInput filter);

		//Task<GetMasterLoLForEditOutput> GetMasterLoLForEdit(EntityDto input);

		//Task CreateOrEdit(CreateOrEditMasterLoLDto input);

		//Task Delete(EntityDto input);

		
		//Task<PagedResultDto<MasterLoLDatabaseLookupTableDto>> GetAllDatabaseForLookupTable(GetAllForLookupTableInput input);
		
    }
}