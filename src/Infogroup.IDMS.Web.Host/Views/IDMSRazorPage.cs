using Abp.AspNetCore.Mvc.Views;

namespace Infogroup.IDMS.Web.Views
{
    public abstract class IDMSRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected IDMSRazorPage()
        {
            LocalizationSourceName = IDMSConsts.LocalizationSourceName;
        }
    }
}
