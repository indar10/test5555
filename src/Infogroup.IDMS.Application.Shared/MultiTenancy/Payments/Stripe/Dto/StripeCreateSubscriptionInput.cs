namespace Infogroup.IDMS.MultiTenancy.Payments.Stripe.Dto
{
    public class StripeCreateSubscriptionInput
    {
        public long PaymentId { get; set; }

        public string StripeToken { get; set; }
    }
}