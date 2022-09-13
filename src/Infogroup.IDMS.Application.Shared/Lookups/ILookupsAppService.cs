using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Lookups.Dtos;
using System.Collections.Generic;
using Infogroup.IDMS.Shared.Dtos;



namespace Infogroup.IDMS.Lookups
{
    public interface ILookupsAppService : IApplicationService 
    {
        PagedResultDto<LookupDto> GetAllLookups(GetAllForLookupTableInput input);

        Task CreateOrEdit(CreateOrEditLookupDto input);

        CreateOrEditLookupDto GetLookupForEdit(int id);

        List<DropdownOutputDto> GetAllLookupsForDropdown();


    }
}