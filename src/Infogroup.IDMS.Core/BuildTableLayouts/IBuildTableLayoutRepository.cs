using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Infogroup.IDMS.MatchAppends.Dtos;
using Infogroup.IDMS.SegmentSelections.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.SICCodes.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.BuildTableLayouts
{
    public interface IBuildTableLayoutRepository : IRepository<BuildTableLayout, int>
    {
        int GetBuildTableLayoutID(int iBuildID, string sFieldName, string cTableName);
        FieldDetails GetSingleFieldDetails(int buildId, int mailerId, int iBuildLoLID, string sFileSpecific, string ID, int iDBID);
        GetExportLayoutSelectedFieldsDto GetFieldName(string field, int tableId);

        bool CheckMaxPerFields(int campaignId, string configItem);

        Task<Field> GetFieldDetailByName(string BuildID, string TablePrefix, string FieldName, int DatabaseID);
        Task<List<DropdownOutputDto>> GetValues(int sBuildLayoutID);

        List<GetBuildTableLayoutForViewDto> GetAllMultiFieldsData(GetAllBuildTableLayoutsInput filter);

        List<string> GetFavouriteFields(int databaseId, int userId);

        Task<List<DropdownOutputDto>> GetFindReplaceFields(int buildId, int databaseId, int mailerId);
        GetMatchAppendFieldDetails GetMatchAppendFieldDetails(string field, int tableId);
        List<DropdownOutputDto> GetBuildTableLayoutFieldByBuildID(string iBuildID);
        List<FieldData> GetFieldsForBuildLayout(int buildId, int mailerId, int iBuildLoLID, string sFileSpecific);
        BuildTableLayoutDto GetExportableField(string fieldName, int BuildTableId);
    }
}
