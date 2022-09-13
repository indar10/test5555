using Abp.Events.Bus;

namespace Infogroup.IDMS.MultiTenancy
{
    public class RecurringPaymentsEnabledEventData : EventData
    {
        public int TenantId { get; set; }
    }
}