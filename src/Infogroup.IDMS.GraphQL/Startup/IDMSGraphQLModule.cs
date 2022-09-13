using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Infogroup.IDMS.Startup
{
    [DependsOn(typeof(IDMSCoreModule))]
    public class IDMSGraphQLModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IDMSGraphQLModule).GetAssembly());
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }
    }
}