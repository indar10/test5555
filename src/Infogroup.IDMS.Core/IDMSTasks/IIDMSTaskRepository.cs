using Abp.Domain.Repositories;
using Infogroup.IDMS.Builds.Dtos;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.IDMSTasks.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
using System.Data;

namespace Infogroup.IDMS.IDMSTasks
{
    public interface IIDMSTaskRepository : IRepository<IDMSTask, int>
    {
        int GetDivionIdFromDatabase(int databaseId);

        DataSet CheckTableName(string connectionString, string sbNewTableName);

        int RenameTables(string connectionString, string oldTableName, string NewTableName);

        int GetBuildLolID(int BuildID, string ListID);

        void DropTableTORename(string connection, string tablename);

        void TaskLoadMailerUsuage(LoadMailerUsageDto input);

        BuildDetails GetDivisionIdFromDatabaseAndBuild(int buildId, int databaseId);

        string GetFieldName(int buildId);

        BuildCopyDto ValidateCopyBuild(int TargetBuildId, int SourceBuildId);

        List<DropdownOutputDto> GetExportFlagFields(int buildID);

        List<DropdownOutputDto> GetAllTableDescription(string buildID);

        int GetDataSetDatabaseByBuildID(int BuildID);

        List<GetAllListForBuildDto> GetAllListForBuild(int BuildID);
    }
    
}
