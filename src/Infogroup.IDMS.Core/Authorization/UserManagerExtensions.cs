using System.Threading.Tasks;
using Abp.Authorization.Users;
using Infogroup.IDMS.Authorization.Users;

namespace Infogroup.IDMS.Authorization
{
    public static class UserManagerExtensions
    {
        public static async Task<User> GetAdminAsync(this UserManager userManager)
        {
            return await userManager.FindByNameAsync(AbpUserBase.AdminUserName);
        }
    }
}
