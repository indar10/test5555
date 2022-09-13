namespace Infogroup.IDMS.OrderStatuss
{
    public static class OrderStatusHelper
    {       
        public static string GetOrderStatusNameByStatus(int OrderStatusID)
        {
            switch ((CampaignStatus)OrderStatusID)
            {
                case CampaignStatus.OrderCreated:
                    return OrderStatusNames.CountCreated;
                case CampaignStatus.OrderSubmitted:
                    return OrderStatusNames.CountSubmitted;
                case CampaignStatus.OrderRunning:
                    return OrderStatusNames.CountRunning;
                case CampaignStatus.OrderCompleted:
                    return OrderStatusNames.CountCompleted;
                case CampaignStatus.OrderFailed:
                    return OrderStatusNames.CountFailed;
                case CampaignStatus.ReadytoOutput:
                    return OrderStatusNames.ReadytoOutput;
                case CampaignStatus.OutputSubmitted:
                    return OrderStatusNames.OutputSubmitted;
                case CampaignStatus.OutputRunning:
                    return OrderStatusNames.OutputRunning;
                case CampaignStatus.OutputCompleted:
                    return OrderStatusNames.OutputCompleted;
                case CampaignStatus.OutputFailed:
                    return OrderStatusNames.OutputFailed;
                case CampaignStatus.ApprovedforShipping:
                    return OrderStatusNames.ApprovedforShipping;
                case CampaignStatus.WaitingtoShip:
                    return OrderStatusNames.WaitingtoShip;
                case CampaignStatus.Shipped:
                     return OrderStatusNames.Shipped;
                case CampaignStatus.ShippingFailed:
                    return OrderStatusNames.ShipmentFailed;
                case CampaignStatus.Cancelled:
                    return OrderStatusNames.Cancelled;
                default:
                    return string.Empty;
            }
        }
    }
}
