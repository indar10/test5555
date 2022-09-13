using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.MatchAppendStatuses.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.MatchAppendStatuses
{
    public interface IMatchAppendStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetMatchAppendStatusForViewDto>> GetAll(GetAllMatchAppendStatusesInput input);

		Task<GetMatchAppendStatusForEditOutput> GetMatchAppendStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditMatchAppendStatusDto input);

		Task Delete(EntityDto input);

		
    }
}