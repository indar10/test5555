using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Editions;
using Infogroup.IDMS.MultiTenancy.Payments;
using Infogroup.IDMS.Sessions.Dto;
using Infogroup.IDMS.UiCustomization;
using Infogroup.IDMS.IDMSConfigurations;

namespace Infogroup.IDMS.Sessions
{
    public class SessionAppService : IDMSAppServiceBase, ISessionAppService
    {
        private readonly IUiThemeCustomizerFactory _uiThemeCustomizerFactory;
        private readonly AppSession _mySession;
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly string BuildVersion = "DataLynxBuildVersion";

        public SessionAppService(
                    IUiThemeCustomizerFactory uiThemeCustomizerFactory,
                    ISubscriptionPaymentRepository subscriptionPaymentRepository,
                    AppSession mySession,
                    IRedisIDMSConfigurationCache idmsConfigurationCache)
        {
            _uiThemeCustomizerFactory = uiThemeCustomizerFactory;
            _mySession = mySession;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
        }

        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var buildVersion = _idmsConfigurationCache.GetConfigurationValue(BuildVersion)?.cValue ?? "1.0";
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = buildVersion,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            var uiCustomizer = await _uiThemeCustomizerFactory.GetCurrentUiCustomizer();
            output.Theme = await uiCustomizer.GetUiSettings();

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper
                    .Map<TenantLoginInfoDto>(await TenantManager
                        .Tenants
                        .Include(t => t.Edition)
                        .FirstAsync(t => t.Id == AbpSession.GetTenantId()));
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
                if (_mySession.IDMSUserId != 0)
                {
                    var idms = new IdmsUserLoginInfoDto
                    {
                        IDMSUserID = _mySession.IDMSUserId,
                        IDMSUserName = _mySession.IDMSUserName                       
                    };
                    output.IdmsUser = idms;
                }
            }

            if (output.Tenant == null)
            {
                return output;
            }

            if (output.Tenant.Edition != null)
            {
                var lastPayment = await _subscriptionPaymentRepository.GetLastCompletedPaymentOrDefaultAsync(output.Tenant.Id, null, null);
                if (lastPayment != null)
                {
                    output.Tenant.Edition.IsHighestEdition = IsEditionHighest(output.Tenant.Edition.Id, lastPayment.GetPaymentPeriodType());
                }
            }

            output.Tenant.SubscriptionDateString = GetTenantSubscriptionDateString(output);
            output.Tenant.CreationTimeString = output.Tenant.CreationTime.ToString("d");

            return output;

        }

        private bool IsEditionHighest(int editionId, PaymentPeriodType paymentPeriodType)
        {
            var topEdition = GetHighestEditionOrNullByPaymentPeriodType(paymentPeriodType);
            if (topEdition == null)
            {
                return false;
            }

            return editionId == topEdition.Id;
        }

        private SubscribableEdition GetHighestEditionOrNullByPaymentPeriodType(PaymentPeriodType paymentPeriodType)
        {
            var editions = TenantManager.EditionManager.Editions;
            if (editions == null || !editions.Any())
            {
                return null;
            }

            return editions.Cast<SubscribableEdition>()
                .OrderByDescending(e => e.GetPaymentAmountOrNull(paymentPeriodType) ?? 0)
                .FirstOrDefault();
        }

        private string GetTenantSubscriptionDateString(GetCurrentLoginInformationsOutput output)
        {
            return output.Tenant.SubscriptionEndDateUtc == null
                ? L("Unlimited")
                : output.Tenant.SubscriptionEndDateUtc?.ToString("d");
        }

        public async Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken()
        {
            if (AbpSession.UserId <= 0)
            {
                throw new Exception(L("ThereIsNoLoggedInUser"));
            }

            var user = await UserManager.GetUserAsync(AbpSession.ToUserIdentifier());
            user.SetSignInToken();
            return new UpdateUserSignInTokenOutput
            {
                SignInToken = user.SignInToken,
                EncodedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString())),
                EncodedTenantId = user.TenantId.HasValue
                    ? Convert.ToBase64String(Encoding.UTF8.GetBytes(user.TenantId.Value.ToString()))
                    : ""
            };
        }
    }
}