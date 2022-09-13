using Abp.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Infogroup.IDMS.Authorization.Roles;
using Infogroup.IDMS.Campaigns;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Infogroup.IDMS.IDMSUsers;

namespace Infogroup.IDMS.Authorization.Users
{
    public class UserClaimsPrincipalFactory : AbpUserClaimsPrincipalFactory<User, Role>
    {
        private readonly ICampaignRepository _cmpRepository;
        private readonly IIDMSUserRepository _userRepository;
        private readonly IRedisIDMSUserCache _userCache;


        public UserClaimsPrincipalFactory(
            UserManager userManager,
            ICampaignRepository cmpRepository,
            IIDMSUserRepository userRepository,
            RoleManager roleManager,
            IRedisIDMSUserCache userCache,
            IOptions<IdentityOptions> optionsAccessor)
            : base(
                  userManager,
                  roleManager,
                  optionsAccessor)
        {
            _cmpRepository = cmpRepository;
            _userRepository = userRepository;
            _userCache = userCache;

        }
        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var idmsUser = _userRepository.GetUserByUserName(user.UserName);
            var claim = await base.CreateAsync(user);
            _userCache.SetDropdownOptions(idmsUser.Id, UserDropdown.Databases);
            _userCache.SetDatabaseIDs(idmsUser.Id);
            _userCache.SetAccessObjects(idmsUser.Id);
            _userCache.SetDatabaseAccessObjects(idmsUser.Id);
            claim.Identities.First().AddClaim(new Claim("Application_UserName", user.UserName));
            claim.Identities.First().AddClaim(new Claim("Application_IdmsUserId", idmsUser.Id.ToString()));
            claim.Identities.First().AddClaim(new Claim("Application_IdmsUserEmail", idmsUser.cEmail.ToString()));
            claim.Identities.First().AddClaim(new Claim("Application_IdmsUserFullName", $@"{idmsUser.cFirstName} {idmsUser.cLastName}"));
            return claim;
        }
    }
}
