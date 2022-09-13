using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.DivisionShipTos.Dtos;

namespace Infogroup.IDMS.DivisionShipTos
{
    public interface IDivisionShipTosAppService : IApplicationService 
    {
        Task<PagedResultDto<GetDivisionShipToForViewDto>> GetAllDivisionalShipTo(GetAllDivisionShipTosInput input);

		Task<CreateOrEditDivisionShipToDto> GetDivisionShipToForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditDivisionShipToDto input);

    }
}