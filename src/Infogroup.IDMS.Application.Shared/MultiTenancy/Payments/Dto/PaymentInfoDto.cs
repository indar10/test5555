using Infogroup.IDMS.Editions.Dto;

namespace Infogroup.IDMS.MultiTenancy.Payments.Dto
{
    public class PaymentInfoDto
    {
        public EditionSelectDto Edition { get; set; }

        public decimal AdditionalPrice { get; set; }
    }
}
