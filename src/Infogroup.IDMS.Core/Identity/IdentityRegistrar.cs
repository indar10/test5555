using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Infogroup.IDMS.Authentication.TwoFactor.Google;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.Authorization.Roles;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Editions;
using Infogroup.IDMS.MultiTenancy;

namespace Infogroup.IDMS.Identity
{
    public static class IdentityRegistrar
    {
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();

            return services.AddAbpIdentity<Tenant, User, Role>(options =>
                {
                    options.Tokens.ProviderMap[GoogleAuthenticatorProvider.Name] = new TokenProviderDescriptor(typeof(GoogleAuthenticatorProvider));
                })
                .AddAbpTenantManager<TenantManager>()
                .AddAbpUserManager<UserManager>()
                .AddAbpRoleManager<RoleManager>()
                .AddAbpEditionManager<EditionManager>()
                .AddAbpUserStore<UserStore>()
                .AddAbpRoleStore<RoleStore>()
                .AddAbpSignInManager<SignInManager>()
                .AddAbpUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddAbpSecurityStampValidator<SecurityStampValidator>()
                .AddPermissionChecker<PermissionChecker>()
                .AddDefaultTokenProviders();
        }
    }
}
