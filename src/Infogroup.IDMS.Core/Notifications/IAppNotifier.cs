using System;
using System.Threading.Tasks;
using Abp;
using Abp.Notifications;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.MultiTenancy;

namespace Infogroup.IDMS.Notifications
{
    public interface IAppNotifier
    {
        Task WelcomeToTheApplicationAsync(User user);

        Task NewUserRegisteredAsync(User user);

        Task NewTenantRegisteredAsync(Tenant tenant);

        Task GdprDataPrepared(UserIdentifier user, Guid binaryObjectId);

        Task SendMessageAsync(UserIdentifier user, string message, NotificationSeverity severity = NotificationSeverity.Info);

        Task TenantsMovedToEdition(UserIdentifier argsUser, string sourceEditionName, string targetEditionName);

        Task SomeUsersCouldntBeImported(UserIdentifier argsUser, string fileToken, string fileType, string fileName);
    }
}
