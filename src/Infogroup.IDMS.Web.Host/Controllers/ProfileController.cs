using Abp.AspNetCore.Mvc.Authorization;
using Infogroup.IDMS.Storage;

namespace Infogroup.IDMS.Web.Controllers
{
    [AbpMvcAuthorize]
    public class ProfileController : ProfileControllerBase
    {
        public ProfileController(ITempFileCacheManager tempFileCacheManager) :
            base(tempFileCacheManager)
        {
        }
    }
}