using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Infogroup.IDMS.Configure;
using Infogroup.IDMS.Startup;
using Infogroup.IDMS.Test.Base;

namespace Infogroup.IDMS.GraphQL.Tests
{
    [DependsOn(
        typeof(IDMSGraphQLModule),
        typeof(IDMSTestBaseModule))]
    public class IDMSGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IDMSGraphQLTestModule).GetAssembly());
        }
    }
}