using System.Threading.Tasks;
using Abp.Application.Services;

namespace Infogroup.IDMS.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
    }
}
