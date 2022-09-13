using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.UserDatabases.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.UserDatabases
{
    public interface IUserDatabasesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserDatabaseForViewDto>> GetAll(GetAllUserDatabasesInput input);

		Task<GetUserDatabaseForEditOutput> GetUserDatabaseForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUserDatabaseDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<UserDatabaseUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<UserDatabaseDatabaseLookupTableDto>> GetAllDatabaseForLookupTable(GetAllForLookupTableInput input);
		
    }
}