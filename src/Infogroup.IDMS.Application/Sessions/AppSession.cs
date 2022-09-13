using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime;
using Abp.Runtime.Session;
using System.Linq;
using System;

namespace Infogroup.IDMS.Sessions
{
    public class AppSession : ClaimsAbpSession, ITransientDependency, IAppSession
    {
        public AppSession(
            IPrincipalAccessor principalAccessor,
            IMultiTenancyConfig multiTenancy,
            ITenantResolver tenantResolver,
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider) :
            base(principalAccessor, multiTenancy, tenantResolver, sessionOverrideScopeProvider)
        {

        }

        public int IDMSUserId
        {
            get
            {
                var userIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Application_IdmsUserId");
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return 0;
                }

                return Convert.ToInt32(userIdClaim.Value);
            }
        }
        public string IDMSUserName
        {
            get
            {
                var userNameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Application_UserName");
                if (string.IsNullOrEmpty(userNameClaim?.Value))
                {
                    return null;
                }

                return userNameClaim.Value;
            }
        }


        public string IDMSUserEmail
        {
            get
            {
                var userEmailClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Application_IdmsUserEmail");
                if (string.IsNullOrEmpty(userEmailClaim?.Value))
                {
                    return null;
                }

                return userEmailClaim.Value;
            }
        }

        public string IDMSUserFullName
        {
            get
            {
                var userFullNameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Application_IdmsUserFullName");
                if (string.IsNullOrEmpty(userFullNameClaim?.Value))
                {
                    return null;
                }

                return userFullNameClaim.Value;
            }
        }
    }
}