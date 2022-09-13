using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.Editions.Dto;
using Infogroup.IDMS.MultiTenancy.Dto;

namespace Infogroup.IDMS.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}