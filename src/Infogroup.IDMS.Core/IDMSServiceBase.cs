using Abp;

namespace Infogroup.IDMS
{
    /// <summary>
    /// This class can be used as a base class for services in this application.
    /// It has some useful objects property-injected and has some basic methods most of services may need to.
    /// It's suitable for non domain nor application service classes.
    /// For domain services inherit <see cref="IDMSDomainServiceBase"/>.
    /// For application services inherit IDMSAppServiceBase.
    /// </summary>
    public abstract class IDMSServiceBase : AbpServiceBase
    {
        protected IDMSServiceBase()
        {
            LocalizationSourceName = IDMSConsts.LocalizationSourceName;
        }
    }
}