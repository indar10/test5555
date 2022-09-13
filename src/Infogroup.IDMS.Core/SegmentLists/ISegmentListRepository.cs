using Abp.Domain.Repositories;
using Infogroup.IDMS.SegmentLists.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SegmentLists
{
    public interface ISegmentListRepository: IRepository<SegmentList, int>
    {
        Task AddSourcesAsync(int id, string selectedListIDs, string sInitiatedBy, bool isSubSelect = false);
        Task<List<SourceDto>> GetApprovedSourcesAsync(Tuple<string, List<SqlParameter>> query);
        Task DeleteAsync(int iSegmentID, string selectedListIDs);           
    }
}
