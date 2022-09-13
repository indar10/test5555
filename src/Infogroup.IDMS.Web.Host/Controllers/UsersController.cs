using Abp.AspNetCore.Mvc.Authorization;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.Storage;
using Abp.BackgroundJobs;

namespace Infogroup.IDMS.Web.Controllers
{
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
            : base(binaryObjectManager, backgroundJobManager)
        {
        }
    }
}