using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Managers.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Managers
{
    public interface IManagersAppService : IApplicationService 
    {
        PagedResultDto<ManagerDto> GetAllManagers(GetAllSetupInput input);

		Task<CreateOrEditManagerDto> GetManagerForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditManagerDto input);

		FileDto ExportManagersToExcel(GetAllSetupInput input);

    }
}