using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.SegmentPrevOrderses.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.SegmentPrevOrderses
{
    public interface ISegmentPrevOrdersesAppService : IApplicationService 
    {
        SegmentPrevOrdersView GetExistingPreviousOrders(int campaignID, int segmentID);
        Task<List<GetSegmentPrevOrdersForViewDto>> GetAllPrevOrdersList(GetPreviousOrdersFilters filters);
        Task SavePreviousOrders(CreateOrEditSegmentPrevOrdersDto input);
        List<string> GetListOfSegmentIDs(int previousOrderID);
    }
}