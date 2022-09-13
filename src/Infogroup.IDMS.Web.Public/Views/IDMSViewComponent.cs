using Abp.AspNetCore.Mvc.ViewComponents;

namespace Infogroup.IDMS.Web.Public.Views
{
    public abstract class IDMSViewComponent : AbpViewComponent
    {
        protected IDMSViewComponent()
        {
            LocalizationSourceName = IDMSConsts.LocalizationSourceName;
        }
    }
}