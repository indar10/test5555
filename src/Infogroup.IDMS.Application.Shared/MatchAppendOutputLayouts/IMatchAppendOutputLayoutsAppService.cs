using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.MatchAppendOutputLayouts.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.MatchAppendOutputLayouts
{
    public interface IMatchAppendOutputLayoutsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetMatchAppendOutputLayoutForViewDto>> GetAll(GetAllMatchAppendOutputLayoutsInput input);

		Task<GetMatchAppendOutputLayoutForEditOutput> GetMatchAppendOutputLayoutForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditMatchAppendOutputLayoutDto input);

		Task Delete(EntityDto input);

		
    }
}