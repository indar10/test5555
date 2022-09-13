using System.Collections.Generic;
using Abp.Application.Services;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Divisions
{
    public interface IDivisionsAppService : IApplicationService 
    {
        List<DropdownOutputDto> GetDivisionDropdownsForUser();
    }
}