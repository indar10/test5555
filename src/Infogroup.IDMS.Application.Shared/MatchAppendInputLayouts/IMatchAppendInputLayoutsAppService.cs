using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.MatchAppendInputLayouts.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.MatchAppendInputLayouts
{
    public interface IMatchAppendInputLayoutsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetMatchAppendInputLayoutForViewDto>> GetAll(GetAllMatchAppendInputLayoutsInput input);

		Task<GetMatchAppendInputLayoutForEditOutput> GetMatchAppendInputLayoutForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditMatchAppendInputLayoutDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<MatchAppendInputLayoutMatchAppendLookupTableDto>> GetAllMatchAppendForLookupTable(GetAllForLookupTableInput input);
		
    }
}