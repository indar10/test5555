using Microsoft.Extensions.Configuration;

namespace Infogroup.IDMS.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
