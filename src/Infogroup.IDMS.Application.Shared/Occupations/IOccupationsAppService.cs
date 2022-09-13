using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.Occupations.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Occupations
{
    public interface IOccupationsAppService
    {
        List<DropdownOutputDto> GetAllOccupationByIndustry(string industryCode);
        List<DropdownOutputDto> GetAllSpecialtyTitleByIndustryOccupation(string industryCode, string occupationCode);
        Task<GetOccupationForViewDto> GetInitialData(GetAdvanceFieldDetailsInputDto input);
    }
}