using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.Install.Dto;

namespace Infogroup.IDMS.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}