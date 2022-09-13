using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.UserDatabaseAccessObjects.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.UserDatabaseAccessObjects
{
    public interface IUserDatabaseAccessObjectsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserDatabaseAccessObjectForViewDto>> GetAll(GetAllUserDatabaseAccessObjectsInput input);

		Task<GetUserDatabaseAccessObjectForEditOutput> GetUserDatabaseAccessObjectForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUserDatabaseAccessObjectDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<UserDatabaseAccessObjectIDMSUserLookupTableDto>> GetAllIDMSUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<UserDatabaseAccessObjectAccessObjectLookupTableDto>> GetAllAccessObjectForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<UserDatabaseAccessObjectDatabaseLookupTableDto>> GetAllDatabaseForLookupTable(GetAllForLookupTableInput input);
		
    }
}