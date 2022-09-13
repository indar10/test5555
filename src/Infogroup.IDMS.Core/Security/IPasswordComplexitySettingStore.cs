using System.Threading.Tasks;

namespace Infogroup.IDMS.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
