using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Infogroup.IDMS.Authorization;

namespace Infogroup.IDMS
{
    /// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(
        typeof(IDMSCoreModule)
        )]
    public class IDMSApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IDMSApplicationModule).GetAssembly());
        }
    }
}