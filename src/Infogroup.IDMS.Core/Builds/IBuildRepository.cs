using Abp.Domain.Repositories;
using Infogroup.IDMS.Builds.Dtos;
using Infogroup.IDMS.SegmentSelections.Dtos;
using System.Collections.Generic;
using System.Data;

namespace Infogroup.IDMS.Builds
{
    public interface IBuildRepository : IRepository<Build, int>
    {
        List<ValueList> GetValues(string sBuildLayoutID, int iBuildLoLID, string sOrderBy, string sSortDirection);
        List<FieldData> GetExternalDatabaseFields(int databaseId);
        List<FieldData> GetExternalTablesByOrderIdForSubSelect(int databaseId, int mailerId, int iBuildLoLID, string sFileSpecific);
        int GetSubSelBuildLolID(int iSubSelID, int buildId);
        int GetBuildLolID(int segmentId, int buildId);
        BuildHierarchyDto GetBuildHierarchyDetails(int buildId);
    }
}
