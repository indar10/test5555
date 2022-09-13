using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.States.Dtos;

namespace Infogroup.IDMS.States
{
    public interface IStateAppService : IApplicationService 
    {
        Task<GetStateForViewDto> GetState(GetAdvanceFieldDetailsInputDto input);
        List<DropdownOutputDto> GetCounty(string cStateCode, int databaseID);
        List<DropdownOutputDto> GetCity(string cStateCode, string cCountyCode, int databaseID);
    }
}