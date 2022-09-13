using Abp.Authorization;
using Infogroup.IDMS.Authorization.Roles;
using Infogroup.IDMS.Authorization.Users;

namespace Infogroup.IDMS.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
