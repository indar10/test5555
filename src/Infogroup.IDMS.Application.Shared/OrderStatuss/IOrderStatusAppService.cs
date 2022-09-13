using System.Collections.Generic;
using System.Threading.Tasks;
using Infogroup.IDMS.OrderStatuss.Dtos;

namespace Infogroup.IDMS.OrderStatuss
{
    public interface IOrderStatusAppService
    {
        List<CampaignStatusDto> GetStatusHistory(int campaignId);
        Task UpdateCampaignStatus(int campaignID);
        List<string> GetLastLogStatement(int campaignID, int databaseID);

    }
}