using Abp.Dependency;
using Abp.Reflection.Extensions;
using Microsoft.Extensions.Configuration;
using Infogroup.IDMS.Configuration;

namespace Infogroup.IDMS.Test.Base.Configuration
{
    public class TestAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }

        public TestAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(
                typeof(IDMSTestBaseModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }
    }
}
