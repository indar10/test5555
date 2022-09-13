using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.UI;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.IDMSConfigurations.Dtos;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.ShortSearch;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Infogroup.IDMS.IDMSConfigurations
{
    public partial class IDMSConfigurationsAppService : IDMSAppServiceBase, IIDMSConfigurationsAppService
    {
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IIDMSConfigurationRepository _idmsConfigurationRepository;
        private readonly AppSession _mySession;
        private readonly Utils _utils;
        private readonly IShortSearch _shortSearch;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public IDMSConfigurationsAppService(
            IRedisIDMSConfigurationCache idmsConfigurationCache,
            IIDMSConfigurationRepository idmsConfigurationRepository,
            AppSession mySession,
            Utils util,
            IShortSearch shortSearch,
            IUnitOfWorkManager unitOfWorkManager
            )
        {            
            _idmsConfigurationCache = idmsConfigurationCache;
            _idmsConfigurationRepository = idmsConfigurationRepository;
            _mySession = mySession;
            _utils = util;
            _shortSearch = shortSearch;
            _unitOfWorkManager = unitOfWorkManager;
        }

        #region Get All Configuration List
        public PagedResultDto<GetAllConfigurationsForViewDto> GetAllConfiguration(GetAllConfigurationsListInput input)
        {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var shortWhere = _shortSearch.GetWhere(PageID.IDMSConfiguration, input.Filter);
                var query = GetAllConfigurationsListQuery(input,shortWhere);
                var result = _idmsConfigurationRepository.GetAllConfigurationListResult(query);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Get Configuration For Edit
        public async Task<CreateOrEditConfigurationDto> GetConfigurationForEdit(EntityDto input)
        {
            try
            {
                var configEditData = await _idmsConfigurationRepository.FirstOrDefaultAsync(input.Id);
                var idmsConfigEditData = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CreateOrEditConfigurationDto>(configEditData));
                return idmsConfigEditData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Create Or Edit Configuration
        [AbpAuthorize(AppPermissions.Pages_IDMSConfiguration_Create)]
        public async Task CreateOrEditIDMSConfig(CreateOrEditConfigurationDto input)
        {

            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    if (input.iIsEncrypted)
                    {
                        input.cValue = _utils.IDMSEncrypt(input.cValue);
                    }
                    var idmsConfiguration = ObjectMapper.Map<IDMSConfiguration>(input);
                    await _idmsConfigurationRepository.InsertAsync(idmsConfiguration);
                }
                else
                {
                    var updateConfig = _idmsConfigurationRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    if (!input.iIsEncrypted && updateConfig.iIsEncrypted)
                    {
                        input.cValue = Utils.IDMSDecrypt(input.cValue);
                    }
                    ObjectMapper.Map(input, updateConfig);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Get cItems For Dropdown
        public List<DropdownOutputDto> GetConfigurationItems()
        {
            try
            {
                return _idmsConfigurationRepository.GetItemsForDropdown();
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Configuration Bizness

        private static Tuple<string, string, List<SqlParameter>> GetAllConfigurationsListQuery(GetAllConfigurationsListInput filters,string shortWhere)
        {
            string firstPasswordString = "%pwd%";
            string secondPasswordString = "%password%";
            var query = new Common.QueryBuilder();
            query.AddSelect($"tblconfig.ID, tblconfig.DivisionID,tblconfig.DatabaseID, db.cDatabaseName, cItem, cDescription,left(cValue, 80) cValue,iValue, dValue, mValue, iIsActive, tblconfig.cCreatedBy, tblconfig.cModifiedBy, tblconfig.dCreatedDate, tblconfig.dModifiedDate");
            query.AddFrom("tblConfiguration", "tblconfig");
            query.AddJoin("tblDatabase", "db", "DatabaseID", "tblconfig", "LEFT JOIN", "ID");
            query.AddWhere("", "cValue", "NOT LIKE", firstPasswordString);
            query.AddWhere("AND", "cValue", "NOT LIKE", secondPasswordString);
            if (shortWhere.Length > 0)
                query.AddWhereString($"AND({shortWhere})");
            query.AddSort(filters.Sorting ?? "tblconfig.ID DESC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }
        public IDMSConfigurationDto GetConfigurationValue(string cItem, int databaseId)
        {
            return ObjectMapper.Map<IDMSConfigurationDto>(_idmsConfigurationCache.GetConfigurationValue(cItem, databaseId));
        }

        public IEnumerable<IDMSConfigurationDto> GetMultipleConfigurationsValue(List<string> cItem, int databaseId)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin()) // Fix for Exception: Cannot access a disposed object
            {
                for (var i = 0; i < cItem.Count; i++)
                {
                    yield return GetConfigurationValue(cItem[i], databaseId);
                }
            }
        }
        #endregion

    }
}
