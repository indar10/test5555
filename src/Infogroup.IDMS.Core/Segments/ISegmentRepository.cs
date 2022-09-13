using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Segments.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Segments
{
    public interface ISegmentRepository : IRepository<Segment, int>
    {
        Task<PagedResultDto<GetSegmentListForView>> GetAllSegmentsList(GetSegmentListInput input, string query, string query1, List<SqlParameter> sqlParams);
        int GetSegmentListCount(int segmentId);
        Task DeleteSegmentAsync(int iSegmentId);
        Task<int> CopySegmentAsync(CopySegmentDto copySegmentInfo);
        Task<ImportSegmentDTO> ImportSegmentValidationAsync(int iCopyToOrderID, int iCopyFromOrderID, int iUserID);
        Task<int> CopySegmentFromCampaign(int iCopyToOrderID, int iCopyFromOrderID, string sCommanSeparatedSegments, string sInitiatedBy);
        bool IsOrderInStatusToUpdate(int iSegmentID);       
        void MoveSegment(int segmentId, int fromSegment, int toSegment, int toLocation, string initiatedBy);
        Task<PagedResultDto<SegmentsGlobalChangesDto>> GetAllSegmentsForGlobalChanges(Tuple<string, string, List<SqlParameter>> query);
        Task<List<int>> GetAllSegmentIDsForGlobalChanges(Tuple<string, string, List<SqlParameter>> query);
        Task<GetSegmentListForView> GetSegmentsForViewById(Tuple<string, List<SqlParameter>> query);
        List<BatchEditSegmentDto> GetAllSegmentsForBatchEdit(Tuple<string, string, List<SqlParameter>> query);
    }
}
