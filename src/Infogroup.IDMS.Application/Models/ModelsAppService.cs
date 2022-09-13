using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Models.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Infogroup.IDMS.Databases;
using Abp.UI;
using System.Data.SqlClient;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.ModelDetails;
using Infogroup.IDMS.ModelQueues;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.UserDatabases;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.ModelDetails.Dtos;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.Common;

namespace Infogroup.IDMS.Models
{
    [AbpAuthorize]
    public partial class ModelsAppService : IDMSAppServiceBase, IModelsAppService
    {
        private readonly AppSession _mySession;
        private readonly IRepository<Database, int> _databaseRepository;
        private readonly IModelsRepository _customModelRepository;
        private readonly IRepository<ModelDetail> _modelDetailsRepository;
        private readonly IRepository<ModelQueue> _modelQueueRepository;
        private readonly IRepository<Build> _buildRepository;
        private readonly IRepository<UserDatabase> _userDatabaseRepository;
        private readonly IRepository<Lookup, int> _lookupRepository;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IRedisIDMSUserCache _userCache;
        private readonly IShortSearch _shortSearch;
        private readonly IIDMSPermissionChecker _permissionChecker;


        public ModelsAppService(
            AppSession mySession,
            IRepository<Database, int> databaseRepository,
            IModelsRepository customModelRepository,
            IRepository<ModelDetail> modelDetailsRepository,
            IRepository<ModelQueue> modelQueueRepository,
            IRepository<Build> buildRepository,
            IRepository<UserDatabase> userDatabaseRepository,
            IRepository<Lookup> lookupRepository,
            IRedisIDMSConfigurationCache idmsConfigurationCache,
            IShortSearch shortSearch,
            IRedisIDMSUserCache userCache,
            IIDMSPermissionChecker permissionChecker
            ) 
		  {
            _mySession = mySession;
            _databaseRepository = databaseRepository;
            _customModelRepository = customModelRepository;
            _modelDetailsRepository = modelDetailsRepository;
            _modelQueueRepository = modelQueueRepository;
            _buildRepository = buildRepository;
            _userDatabaseRepository = userDatabaseRepository;
            _userCache = userCache;
            _lookupRepository = lookupRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
            _shortSearch = shortSearch;
            _permissionChecker = permissionChecker;
        }

        #region Fetch Models
        public PagedResultDto<ModelScoringDto> GetAllModels(GetAllModelsInput input)
         {
            try
            {
                input.Filter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                var DatabaseIds = _userCache.GetDatabaseIDs(_mySession.IDMSUserId);
                var shortWhere = _shortSearch.GetWhere(PageID.Models, input.Filter);
                var query = GetAllModelsQuery(input, _mySession.IDMSUserId, DatabaseIds, shortWhere);
                return _customModelRepository.GetAllModelsList(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public GetModelTypeAndWeightDto GetModelTypeAndWeight()
        {
            GetModelTypeAndWeightDto modelTypeAndWeightDto = new GetModelTypeAndWeightDto();
            modelTypeAndWeightDto.ModelType = _lookupRepository.GetAll()
                                                .Where(item => item.cLookupValue == ModelsConsts.ModelType)
                                                .Select(item => new DropdownOutputDto { Label = item.cDescription, Value = item.cCode }).ToList();
            modelTypeAndWeightDto.ModelGiftWeight = _lookupRepository.GetAll()
                                                .Where(item => item.cLookupValue == ModelsConsts.ModelGiftWeight)
                                                .Select(item => new DropdownOutputDto { Label = item.cDescription, Value = item.cCode }).ToList();
            return modelTypeAndWeightDto;
        }
        #endregion

        #region save models
        [AbpAuthorize(AppPermissions.Pages_Models_Create)]
        public async Task CreateAsync(CreateOrEditModelDto input)
         {
            try
            {
                if(!input.IsCopyModel)
                    ValidateModel(input.ModelSummaryData.cModelName,input.ModelDetailData.ModelID);
                else
                {
                    var isExistingBuild = _modelDetailsRepository.GetAll().Any(x => x.BuildID == input.ModelDetailData.BuildID && x.ModelID == input.ModelDetailData.ModelID);
                    if (isExistingBuild) throw new UserFriendlyException(L("ValidateSameBuild"));
                }
                var inputModelDetail = CommonHelpers.ConvertNullStringToEmptyAndTrim(input.ModelDetailData);
                var inputModelSummary = CommonHelpers.ConvertNullStringToEmptyAndTrim(input.ModelSummaryData);
                input.ModelDetailData = inputModelDetail;
                input.ModelSummaryData = inputModelSummary;
                if (input.Id == 0 || input.IsCopyModel)
                {
                    if (!input.IsCopyModel)
                        input.ModelDetailData.ModelID = await CreateModelSummary(input.ModelSummaryData);
                    else                    
                        UpdateModelSummary(input.ModelSummaryData, input.ModelDetailData.ModelID);                    

                    var modelDetailId = await CreateModelDetail(input.ModelDetailData);                                        
                    await CreateModelQueue(modelDetailId);
                }
                else
                {
                    UpdateModelSummary(input.ModelSummaryData, input.ModelDetailData.ModelID);
                    UpdateModelDetail(input.ModelDetailData,input.Id);
                }
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
            
         }
        #endregion

        #region Get Model For edit
        public CreateOrEditModelDto GetModelForEdit(int modelDetailId, bool isCopy)
        {
            try
            {
                var query = GetModelsById(modelDetailId);
                var modelScore = _customModelRepository.GetModelById(query);
                return ObjectMapper.Map<CreateOrEditModelDto>(modelScore);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            
        }
        #endregion

        #region Validation
        private void ValidateModel(string ModelName,int ModelSummaryId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ModelName))
                {
                    var isExistingModelName = _customModelRepository.GetAll().Any(p => p.cModelName.ToLower() == ModelName.ToLower() && p.Id != ModelSummaryId);
                    if (isExistingModelName) throw new UserFriendlyException(L("ValidateModelName"));
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }            
        }
        #endregion

        #region Model Scoring Forms
        public GetModelScoringDropdownDto GetDatabaseWithLatestBuild(int defaultDatabaseId)
        {
            try
            {
                var databases = GetAllDatabasesForDropdown(_mySession.IDMSUserId, defaultDatabaseId);
                var result = GetFieldsOnDatabaseChange(databases.DefaultDatabase);
                result.Databases = databases;
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public GetAllDatabaseDto GetAllDatabasesForDropdown(int userid, int currentDatabaseId)
        {
            try
            {
                var defaultSelection = currentDatabaseId > 0 ? currentDatabaseId : _userCache.GetDefaultDatabaseForCampaign(userid, Convert.ToInt32(_mySession.UserId));
               
                var dropdownValues = _userCache.GetDropdownOptions(_mySession.IDMSUserId, UserDropdown.Databases);
                return new GetAllDatabaseDto
                {
                    DefaultDatabase = defaultSelection,
                    Databases = dropdownValues
                };

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public GetModelScoringDropdownDto GetFieldsOnDatabaseChange(int iDatabaseID)
        {
            try
            {
                var result = new GetModelScoringDropdownDto
                {
                    Builds = GetBuildsForDatabase(iDatabaseID),                   
                };
                
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        private GetAllBuildsDto GetBuildsForDatabase(int iDatabaseID)
        {
            try
            {
                var result = new GetAllBuildsDto
                {
                    DefaultSelection = (from build in _buildRepository.GetAll()
                                        join userDatabase in _userDatabaseRepository.GetAll()
                                        on build.DatabaseId equals userDatabase.DatabaseId
                                        where build.iIsReadyToUse
                                        && build.iIsOnDisk
                                        && userDatabase.UserId == _mySession.IDMSUserId
                                        && build.DatabaseId == iDatabaseID
                                        orderby build.Id descending
                                        select build.Id).FirstOrDefault()
                };
                if (_permissionChecker.IsGranted(_mySession.IDMSUserId, PermissionList.RunCountsInactiveBuild, AccessLevel.iAddEdit))
                {
                    result.BuildDropDown = (from build in _buildRepository.GetAll()
                                            join userDatabase in _userDatabaseRepository.GetAll()
                                            on build.DatabaseId equals userDatabase.DatabaseId
                                            where build.iIsReadyToUse
                                            && userDatabase.UserId == _mySession.IDMSUserId
                                            && build.DatabaseId == iDatabaseID
                                            orderby build.cBuild descending
                                            select new DropdownOutputDto
                                            {
                                                Value = build.Id,
                                                Label = $"{build.cDescription} : {build.Id}"
                                            }).ToList();
                }
                else
                {
                    result.BuildDropDown = (from build in _buildRepository.GetAll()
                                            join userDatabase in _userDatabaseRepository.GetAll()
                                            on build.DatabaseId equals userDatabase.DatabaseId
                                            where build.iIsReadyToUse
                                            && build.iIsOnDisk
                                            && userDatabase.UserId == _mySession.IDMSUserId
                                            && build.DatabaseId == iDatabaseID
                                            orderby build.cBuild descending
                                            select new DropdownOutputDto
                                            {
                                                Value = build.Id,
                                                Label = $"{build.cDescription} : {build.Id}"
                                            }).ToList();
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        #endregion

        #region Business Logic
        private static Tuple<string, string, List<SqlParameter>> GetAllModelsQuery(GetAllModelsInput filters,int CurrentUserId, List<int> DatabaseIds, string shortWhere)
        {

            if (shortWhere != null & shortWhere.Length > 0)
                filters.Filter = "";

            if (!string.IsNullOrEmpty(filters.Filter))
                filters.Filter = filters.Filter.Trim();

            string[] filtersarray = null;
            var isModelId = Validation.ValidationHelper.IsNumeric(filters.Filter);

            string[] DbIds = DatabaseIds.Select(x => x.ToString()).ToArray();

            if (!string.IsNullOrEmpty(filters.Filter))
                filtersarray = filters.Filter.Split(',');            

            var defaultFilter = $@"AND (M.cModelName LIKE @FilterText OR D.cDatabaseName LIKE @FilterText OR B.cDescription LIKE @FilterText)";

            var query = new Common.QueryBuilder();
            query.AddSelect($"MD.ID,MD.ModelID,M.cModelName,M.cModelNumber,M.cDescription as 'Model Description',D.cDatabaseName as 'DatabaseName',B.cDescription as 'Build',L.cDescription as Status,MQ.LK_ModelStatus, MQ.dCreatedDate as 'StatusDate',M.iIsActive, LMT.cDescription as ModelType");
            query.AddFrom("tblModelDetail", "MD");
            query.AddJoin("tblModel", "M", "ModelID", "MD", "INNER JOIN", "ID");
            query.AddJoin("tblDatabase", "D", "DatabaseID", "M", "INNER JOIN", "ID");
            query.AddJoin("tblBuild", "B", "BuildID", "MD", "INNER JOIN", "ID");
            query.AddJoin("tblUserDatabase", "UD", "ID", "D", "INNER JOIN", "DatabaseID");
            query.AddJoin("tblModelQueue", "MQ", "ID", "MD", "INNER JOIN", "ModelDetailID").And("MQ.iIsCurrent", "EQUALTO", "1");
            query.AddJoin("tblLookup", "L", "LK_ModelStatus", "MQ", "LEFT JOIN", "cCode").And("L.cLookupValue", "LIKE", "'"+"ModelStatus"+"'");
            query.AddJoin("tblLookup", "LMT", "LK_ModelType", "M", "LEFT JOIN", "cCode").And("LMT.cLookupValue", "LIKE", "'" + "ModelType" + "'");
            query.AddJoin("tblLookup", "LGW", "LK_GiftWeight", "M", "LEFT JOIN", "cCode").And("LGW.cLookupValue", "LIKE", "'" + "ModelGiftWeight" + "'");

            query.AddWhere("", "UD.UserID", "EQUALTO", CurrentUserId.ToString());
            if (isModelId)
                query.AddWhere("AND", "MD.ID", "IN", filtersarray);
            else
                query.AddWhereString(defaultFilter);

            if (shortWhere.Length > 0)
                query.AddWhereString($"AND ({shortWhere})");

            query.AddSort(filters.Sorting ?? "'StatusDate' DESC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }

        private static Tuple<string, string, List<SqlParameter>> GetModelsById(int modelDetailId)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"MD.ID,MD.ModelID,M.cModelName,M.iIsScoredForEveryBuild,M.nChildTableNumber,M.cClientName,M.iIntercept,M.cModelNumber,MD.cSQL_Score,MD.cSQL_Deciles,MD.cSQL_Preselect,MD.cSAS_ScoreFileName,M.cDescription as 'Model Description',D.ID as 'DatabaseID',B.ID as 'BuildID',M.iIsActive,M.cCreatedBy as 'ModelSummaryCreatedBy', M.dCreatedDate as 'ModelSummaryCreatedDate',MD.cCreatedBy as 'ModelDetailCreatedBy', MD.dCreatedDate as 'ModelDetailCreatedDate', M.LK_ModelType, M.LK_GiftWeight");
            query.AddFrom("tblModelDetail", "MD");
            query.AddJoin("tblModel", "M", "ModelID", "MD", "INNER JOIN", "ID");
            query.AddJoin("tblDatabase", "D", "DatabaseID", "M", "INNER JOIN", "ID");
            query.AddJoin("tblBuild", "B", "BuildID", "MD", "INNER JOIN", "ID");
            query.AddWhere("", "MD.ID", "EQUALTO", modelDetailId.ToString());            
            
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();            

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }

        private static Tuple<string, List<SqlParameter>> GetChildTableNumber(int filter)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"ISNULL(MAX(nChildTableNumber),50) + 1");
            query.AddFrom("TBLMODEL", "M");
            query.AddWhere("", "M.DatabaseID", "EQUALTO", filter.ToString());

            (string sqlSelect, List<SqlParameter> sqlParams)  = query.Build();

            return new Tuple<string, List<SqlParameter>>(sqlSelect.ToString(), sqlParams);
        }
        #endregion

        #region Create Model Summary
        private async Task<int> CreateModelSummary(ModelSummaryDto input)
        {
            try
            {                
                var query = GetChildTableNumber(input.DatabaseId);
                input.nChildTableNumber = _customModelRepository.GetChildTableNumber(query);
                input.cCreatedBy = _mySession.IDMSUserName;
                input.dCreatedDate = DateTime.Now;
                var model=ObjectMapper.Map<Model>(input);
                    
                var modelid = await _customModelRepository.InsertAndGetIdAsync(model);

                CurrentUnitOfWork.SaveChanges();

                return modelid;
            }
            catch (Exception ex)
            {
               throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Create Model Details
        private async Task<int> CreateModelDetail(ModelDetailDto input)
        {
            try
            {                
                input.cCreatedBy = _mySession.IDMSUserName;
                input.dCreatedDate = DateTime.Now;

                var detail = ObjectMapper.Map<ModelDetail>(input);

                var modelDetailId = await _modelDetailsRepository.InsertAndGetIdAsync(detail);

                CurrentUnitOfWork.SaveChanges();

                return modelDetailId;                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Create Model Queue
        private async Task CreateModelQueue(int modelId)
        {
            try
            {
                var queue = new ModelQueue
                {
                    ModelDetailID = modelId,
                    iIsCurrent = true,
                    LK_ModelStatus = "10",
                    iIsSampleScore = false,
                    iPriority = 1,
                    cCreatedBy = _mySession.IDMSUserName,
                    cNotes = string.Empty,
                    dCreatedDate = DateTime.Now
                };

                await _modelQueueRepository.InsertAsync(queue);

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Update Model Summary
        private void UpdateModelSummary(ModelSummaryDto input, int modelSummaryId)
        {
            try
            {                
                var model = _customModelRepository.GetAll().FirstOrDefault(x => x.Id == modelSummaryId);

                input.cModifiedBy = _mySession.IDMSUserName;
                input.dModifiedDate = DateTime.Now;
                ObjectMapper.Map(input, model);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Update Model Detail
        private void UpdateModelDetail(ModelDetailDto input,int ID)
        {
            try
            {                
                var detail = _modelDetailsRepository.GetAll().FirstOrDefault(x => x.Id == ID);

                input.cModifiedBy = _mySession.IDMSUserName;
                input.dModifiedDate = DateTime.Now;
                ObjectMapper.Map(input, detail);
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }
}