using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.MultiTenancy;

namespace Infogroup.IDMS.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}