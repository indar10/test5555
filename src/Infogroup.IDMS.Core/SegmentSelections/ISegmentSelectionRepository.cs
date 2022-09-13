using Abp.Domain.Repositories;
using Infogroup.IDMS.SegmentSelections.Dtos;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SegmentSelections
{
    public interface ISegmentSelectionRepository : IRepository<SegmentSelection, int>
    {
        int GetSubSelListCount(int iSubSelID);
        StringBuilder GetZipSelection(string sZipRadiusHashList);
        int GetSegmentListCount(int segmentId);
        List<BindSegmentSelectionDetails> GetSegmentSelectionBySegmentID(int iSegmentID, int iBuildLoLID);
        string GetNotToBeUpperCasedFields(int databaseId);
        int AddSegmentSelection(SegmentSelectionDto segmentSelection, int campaignId);
        void GroupSelection(int segmentID, int maxGroupId, string selections);
        bool DeleteRecords(List<int> listOfSegmentIds, int campaignId);
        List<SelectionDetails> GetSegmentSelectionByOrderID(int orderId, string fieldName, string TablePrefix = null);
        Task<string> ApplyBulkOperations(SaveGlobalChangesInputDto input);
    }
}
