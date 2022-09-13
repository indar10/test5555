using Abp.Domain.Services;

namespace Infogroup.IDMS
{
    public abstract class IDMSDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected IDMSDomainServiceBase()
        {
            LocalizationSourceName = IDMSConsts.LocalizationSourceName;
        }
    }
}
