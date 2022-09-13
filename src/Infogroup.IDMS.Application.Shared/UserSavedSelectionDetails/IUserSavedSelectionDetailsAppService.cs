using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.UserSavedSelectionDetails.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.UserSavedSelectionDetails
{
    public interface IUserSavedSelectionDetailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUserSavedSelectionDetailForViewDto>> GetAll(GetAllUserSavedSelectionDetailsInput input);

		Task<GetUserSavedSelectionDetailForEditOutput> GetUserSavedSelectionDetailForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUserSavedSelectionDetailDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<UserSavedSelectionDetailUserSavedSelectionLookupTableDto>> GetAllUserSavedSelectionForLookupTable(GetAllForLookupTableInput input);
		
    }
}