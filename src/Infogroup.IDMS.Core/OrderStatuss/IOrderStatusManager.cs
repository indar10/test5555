using System;
using System.Threading.Tasks;

namespace Infogroup.IDMS.OrderStatuss
{
    public interface IOrderStatusManager
    {
        Task UpdateOrderStatus(int campaignID, CampaignStatus newStatus, string modifiedBy, string cNotes = "");
        void UpdateScheduleStatus(int campaignID, int newStatus, string modifiedBy, string time, string date);
    }
}