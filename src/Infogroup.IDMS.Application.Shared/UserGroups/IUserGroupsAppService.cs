using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.UserGroups.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.UserGroups
{
    public interface IUserGroupsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserGroupForViewDto>> GetAll(GetAllUserGroupsInput input);

		Task<GetUserGroupForEditOutput> GetUserGroupForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUserGroupDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<UserGroupTblUserLookupTableDto>> GetAllTblUserForLookupTable(GetAllForLookupTableInput input);
		
    }
}