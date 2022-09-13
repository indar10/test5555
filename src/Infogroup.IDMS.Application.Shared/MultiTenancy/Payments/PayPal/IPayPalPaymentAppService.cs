using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.MultiTenancy.Payments.PayPal.Dto;

namespace Infogroup.IDMS.MultiTenancy.Payments.PayPal
{
    public interface IPayPalPaymentAppService : IApplicationService
    {
        Task ConfirmPayment(long paymentId, string paypalOrderId);

        PayPalConfigurationDto GetConfiguration();
    }
}
