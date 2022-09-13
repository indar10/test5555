
namespace Infogroup.IDMS.OrderStatuss
{
    public enum CampaignStatus 
    {
        OrderCreated = 10,
        OrderSubmitted = 20,
        OrderRunning = 30,
        OrderCompleted = 40,
        OrderFailed = 50,
        ReadytoOutput = 60,
        OutputSubmitted = 70,
        OutputRunning = 80,
        OutputCompleted = 90,
        OutputFailed = 100,
        ApprovedforShipping = 110,
        WaitingtoShip = 120,
        Shipped = 130,
        ShippingFailed = 140,
        Cancelled = 150
    }
    public static class OrderStatusNames
    {
        public const string CountCreated = "10: Count Created";
        public const string CountSubmitted = "20: Count Submitted";
        public const string CountRunning = "30: Count Running";
        public const string CountCompleted = "40: Count Completed";
        public const string CountFailed = "50: Count Failed";
        public const string ReadytoOutput = "60: Ready to Output";
        public const string OutputSubmitted = "70: Output Submitted";
        public const string OutputRunning = "80: Output Running";
        public const string OutputCompleted = "90: Output Completed";
        public const string OutputFailed = "100: Output Failed";
        public const string ApprovedforShipping = "110: Approved for Shipping";
        public const string WaitingtoShip = "120: Waiting to Ship";
        public const string Shipped = "130: Shipped";
        public const string ShipmentFailed = "140: Shipment Failed";
        public const string Cancelled = "150: Cancelled";
    }

}
