using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Infogroup.IDMS
{
    public class IDMSClientModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IDMSClientModule).GetAssembly());
        }
    }
}
