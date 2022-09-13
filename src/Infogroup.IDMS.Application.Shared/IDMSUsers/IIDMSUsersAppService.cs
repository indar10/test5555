using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.IDMSUsers.Dtos;

namespace Infogroup.IDMS.IDMSUsers
{
    public interface IIDMSUsersAppService : IApplicationService 
    {
        Task<PagedResultDto<IDMSUserDto>> GetAll(GetAllIDMSUsersInput input);

        Task<GetIDMSUserForViewDto> GetTblUserForView(int id);

		Task<GetIDMSUserForEditOutput> GetTblUserForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditIDMSUserDto input);

		Task Delete(EntityDto input);

		
    }
}