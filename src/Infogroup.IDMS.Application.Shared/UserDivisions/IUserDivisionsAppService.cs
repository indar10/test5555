using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.UserDivisions.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.UserDivisions
{
    public interface IUserDivisionsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserDivisionForViewDto>> GetAll(GetAllUserDivisionsInput input);

		Task<GetUserDivisionForEditOutput> GetUserDivisionForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUserDivisionDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<UserDivisiontblUserLookupTableDto>> GetAlltblUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<UserDivisionDivisionLookupTableDto>> GetAllDivisionForLookupTable(GetAllForLookupTableInput input);
		
    }
}