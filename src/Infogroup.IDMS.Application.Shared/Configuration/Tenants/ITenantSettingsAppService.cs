using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.Configuration.Tenants.Dto;

namespace Infogroup.IDMS.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task ClearLogo();

        Task ClearCustomCss();
    }
}
