using Abp.Domain.Repositories;
using Infogroup.IDMS.SegmentPrevOrderses.Dtos;
using Infogroup.IDMS.SegmentSelections.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SegmentPrevOrderses
{
    public interface ISegmentPreviousOrderRepository : IRepository<SegmentPrevOrders, int>
    {
        Task<List<GetSegmentPrevOrdersForViewDto>> GetAllPreviousOrders(int iDatabaseID, GetPreviousOrdersFilters filters, int userID, string shortWhere, bool isDivisional, string defaultMatchLevel);
        string ValidateKeyColumnWithPrevOrders(string OrderID, string PreviousOrderID);
        List<string> GetValidPreviousCampaigns(Tuple<string, List<SqlParameter>> query);
        string BulkOperationOnCampaignHistory(SaveGlobalChangesInputDto input, string initiatedBy);

    }
}
