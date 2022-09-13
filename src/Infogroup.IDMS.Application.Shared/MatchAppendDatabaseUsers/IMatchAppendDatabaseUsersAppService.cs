using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.MatchAppendDatabaseUsers.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.MatchAppendDatabaseUsers
{
    public interface IMatchAppendDatabaseUsersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetMatchAppendDatabaseUserForViewDto>> GetAll(GetAllMatchAppendDatabaseUsersInput input);

		Task<GetMatchAppendDatabaseUserForEditOutput> GetMatchAppendDatabaseUserForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditMatchAppendDatabaseUserDto input);

		Task Delete(EntityDto input);

		
    }
}