using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.UserAccessObjects.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.UserAccessObjects
{
    public interface IUserAccessObjectsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserAccessObjectForViewDto>> GetAll(GetAllUserAccessObjectsInput input);

		Task<GetUserAccessObjectForEditOutput> GetUserAccessObjectForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUserAccessObjectDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<UserAccessObjectIDMSUserLookupTableDto>> GetAllIDMSUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<UserAccessObjectAccessObjectLookupTableDto>> GetAllAccessObjectForLookupTable(GetAllForLookupTableInput input);
		
    }
}