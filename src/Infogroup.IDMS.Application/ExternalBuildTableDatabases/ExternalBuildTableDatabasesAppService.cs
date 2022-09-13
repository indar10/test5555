
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ExternalBuildTableDatabases.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using System.Data.SqlClient;
using Infogroup.IDMS.Common.Exporting;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Shared.Dtos;
using System.Text;
using Infogroup.IDMS.Sessions;

namespace Infogroup.IDMS.ExternalBuildTableDatabases
{
	[AbpAuthorize(AppPermissions.Pages_ExternalBuildTableDatabases)]
    public class ExternalBuildTableDatabasesAppService : IDMSAppServiceBase, IExternalBuildTableDatabasesAppService
    { 
        private readonly IExternalBuildTableDatabasesRepository _customExternalDbLinksRepository;
        private readonly ICommonExcelExporter _commonExcelExporter;
        private readonly IShortSearch _shortSearch;
        private readonly AppSession _mySession;
        public ExternalBuildTableDatabasesAppService(
            IExternalBuildTableDatabasesRepository customExternalDbLinksRepository,
             ICommonExcelExporter commonExcelExporter,
              IShortSearch shortSearch,
               AppSession mySession) 
		  {
            _customExternalDbLinksRepository = customExternalDbLinksRepository;
            _commonExcelExporter = commonExcelExporter;
            _shortSearch = shortSearch;
            _mySession = mySession;
          }
        #region Fetch Records
        public PagedResultDto<ExternalBuildTableDatabaseForAllDto> GetAllLinks(GetAllForLookupTableInput input,bool exporttoexcel)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var shortWhere = _shortSearch.GetWhere(PageID.ExternalDatabaseLinks, input.Filter);
                var query = GetallExternalDbLinks(input, exporttoexcel,shortWhere);
                return _customExternalDbLinksRepository.GetAllExternaldbLinks(query);

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public List<DropdownOutputDto> GetAllBuildTableDescForDropdown()
        {
            try
            {
                var query = GetTableDescForDropDown();
                return _customExternalDbLinksRepository.GetTableDescription(query);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        public Task<ExternalBuildTableDatabase> GetById(int id)
        {
            var data = _customExternalDbLinksRepository.GetAsync(id);
            return data;
        }
        #endregion
        #region delete record
        [AbpAuthorize(AppPermissions.Pages_ExternalBuildTableDatabases_Write)]
        public async Task deleteRecord(int id)
        {
            await _customExternalDbLinksRepository.DeleteAsync(id);
        }
        #endregion
        #region Export To Excel
        public FileDto ExportAllExternalDbLinksToExcel(GetAllForLookupTableInput input, bool exporttoexcel)
        {
            try
            {

                var data = GetAllLinks(input, exporttoexcel);
                var excelData = data.Items.ToList();
                var fileName = $"ListExternalDatabaseLinks";

                return _commonExcelExporter.AllExportToExternalDatabaseLinks(excelData, fileName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
        #region Create/update External db links

        [AbpAuthorize(AppPermissions.Pages_ExternalBuildTableDatabases_Write)]
        public async Task CreateOrEdit(CreateOrEditExternalBuildTableDatabaseDto input)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);

                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    var masterData = ObjectMapper.Map<ExternalBuildTableDatabase>(input);
                    await _customExternalDbLinksRepository.InsertAsync(masterData);
                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    var updatelist = _customExternalDbLinksRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, updatelist);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
                }
        #endregion
        #region External Build Table databases Bizness
        private Tuple<string, string, List<SqlParameter>> GetallExternalDbLinks(GetAllForLookupTableInput filters, bool exporttoexcel, string shortWhere)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"EX.ID AS ID , DV.cDivisionName AS DivisionName,DV.ID AS DivisionID ,DB.cDatabaseName AS DatabaseName ,EX.DatabaseID AS DatabaseID,BT.cTableDescription AS BuildTableDescription,BT.cTableName AS BuildTableName,BT.ID AS BuildTableID");
            query.AddFrom("tblExternalBuildTableDatabase", "EX");
            query.AddJoin("tblBuildTable", "BT", "BuildTableID", "EX", "INNER JOIN", "ID");
            query.AddJoin("tblDatabase", "DB", "DatabaseID", "EX", "INNER JOIN", "ID");
            query.AddJoin("tblDivision", "DV", "DivisionID", "DB", "INNER JOIN", "ID");
            query.AddSort("EX.ID DESC");
            if (shortWhere.Length > 0)
            {
                query.AddWhereString($"({shortWhere})");
            } 
            if (!exporttoexcel)
            {
                query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            }  
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();


            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);

        }
        private string GetTableDescForDropDown()

        {
            var query = new StringBuilder();
            query.Append($"SELECT BT.BuildID,BT.ID,  ");
            query.Append(" CASE WHEN BT.cTableDescription = '' THEN  'No description' ELSE BT.cTableDescription END AS 'cTableDescription', BT.cTableName   ");
            query.Append("FROM tblBuildTable BT   ");
            query.Append("INNER JOIN tblConfiguration CG ON BT.BuildID = CG.iValue   ");
            query.Append($"where CG.cItem='ExternalTableBuildID'  ");
            query.Append("Order by BT.cTableDescription");
            return query.ToString();


        }
        #endregion
    }
}