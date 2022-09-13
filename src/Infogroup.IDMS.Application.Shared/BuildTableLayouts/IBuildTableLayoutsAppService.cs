using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.BuildTableLayouts.Dtos;

namespace Infogroup.IDMS.BuildTableLayouts
{
    public interface IBuildTableLayoutsAppService : IApplicationService 
    {
        List<GetBuildTableLayoutForViewDto> GetAllMultiFields(GetAllBuildTableLayoutsInput FilterText);

    }
}