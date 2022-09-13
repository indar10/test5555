using Infogroup.IDMS.Databases;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Builds.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Shared.Dtos;
using Abp.UI;
using Infogroup.IDMS.UserDatabases;
using Infogroup.IDMS.Models;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.BuildTables;

namespace Infogroup.IDMS.Builds
{
    [AbpAuthorize]
    public class BuildsAppService : IDMSAppServiceBase, IBuildsAppService
    {
        private readonly IRepository<Build> _buildRepository;
        private readonly IBuildTableRepository _buildTableRepository;
        private readonly IRepository<Database, int> _databaseRepository;
        private readonly IRepository<UserDatabase> _userDatabaseRepository;
        private readonly ICampaignRepository _customCampaignRepository;
        private readonly AppSession _mySession;
        private readonly IIDMSPermissionChecker _permissionChecker;


        public BuildsAppService(
            IRepository<Build> buildRepository, 
            IRepository<Database, int> databaseRepository, 
            IRepository<UserDatabase> userDatabaseRepository,
            ICampaignRepository customCampaignRepository,
            IIDMSPermissionChecker permissionChecker,
            IBuildTableRepository buildTableRepository,
            AppSession mySession)
        {
            _buildRepository = buildRepository;
            _databaseRepository = databaseRepository;
            _customCampaignRepository = customCampaignRepository;
            _mySession = mySession;
            _userDatabaseRepository = userDatabaseRepository;
            _permissionChecker = permissionChecker;
            _buildTableRepository = buildTableRepository;
        }

        public async Task<PagedResultDto<GetBuildForViewDto>> GetAll(GetAllBuildsInput input)
        {

            var filteredBuilds = _buildRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.LK_BuildStatus.Contains(input.Filter) || e.cBuild.Contains(input.Filter) || e.cDescription.Contains(input.Filter) || e.cMailDateFROM.Contains(input.Filter) || e.cMailDateTO.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LK_BuildStatusFilter), e => e.LK_BuildStatus.ToLower() == input.LK_BuildStatusFilter.ToLower().Trim())
                        .WhereIf(input.MiniPreviousBuildIDFilter != null, e => e.iPreviousBuildID >= input.MiniPreviousBuildIDFilter)
                        .WhereIf(input.MaxiPreviousBuildIDFilter != null, e => e.iPreviousBuildID <= input.MaxiPreviousBuildIDFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.cBuildFilter), e => e.cBuild.ToLower() == input.cBuildFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.cDescriptionFilter), e => e.cDescription.ToLower() == input.cDescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MindMailDateFilter != null, e => e.dMailDate >= input.MindMailDateFilter)
                        .WhereIf(input.MaxdMailDateFilter != null, e => e.dMailDate <= input.MaxdMailDateFilter)
                        .WhereIf(input.MiniRecordCountFilter != null, e => e.iRecordCount >= input.MiniRecordCountFilter)
                        .WhereIf(input.MaxiRecordCountFilter != null, e => e.iRecordCount <= input.MaxiRecordCountFilter)
                        .WhereIf(input.iIsReadyToUseFilter > -1, e => Convert.ToInt32(e.iIsReadyToUse) == input.iIsReadyToUseFilter)
                        .WhereIf(input.iIsOnDiskFilter > -1, e => Convert.ToInt32(e.iIsOnDisk) == input.iIsOnDiskFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.cMailDateFROMFilter), e => e.cMailDateFROM.ToLower() == input.cMailDateFROMFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.cMailDateTOFilter), e => e.cMailDateTO.ToLower() == input.cMailDateTOFilter.ToLower().Trim())
                        .WhereIf(input.MindCreatedDateFilter != null, e => e.dCreatedDate >= input.MindCreatedDateFilter)
                        .WhereIf(input.MaxdCreatedDateFilter != null, e => e.dCreatedDate <= input.MaxdCreatedDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.cCreatedByFilter), e => e.cCreatedBy.ToLower() == input.cCreatedByFilter.ToLower().Trim())
                        .WhereIf(input.MindModifiedDateFilter != null, e => e.dModifiedDate >= input.MindModifiedDateFilter)
                        .WhereIf(input.MaxdModifiedDateFilter != null, e => e.dModifiedDate <= input.MaxdModifiedDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.cModifiedByFilter), e => e.cModifiedBy.ToLower() == input.cModifiedByFilter.ToLower().Trim())
                        .WhereIf(input.MinLK_BuildPriorityFilter != null, e => e.LK_BuildPriority >= input.MinLK_BuildPriorityFilter)
                        .WhereIf(input.MaxLK_BuildPriorityFilter != null, e => e.LK_BuildPriority <= input.MaxLK_BuildPriorityFilter)
                        .WhereIf(input.MindScheduledDateTimeFilter != null, e => e.dScheduledDateTime >= input.MindScheduledDateTimeFilter)
                        .WhereIf(input.MaxdScheduledDateTimeFilter != null, e => e.dScheduledDateTime <= input.MaxdScheduledDateTimeFilter)
                        .WhereIf(input.iStopRequestedFilter > -1, e => Convert.ToInt32(e.iStopRequested) == input.iStopRequestedFilter)
                        .WhereIf(input.iIsOneStepFilter > -1, e => Convert.ToInt32(e.iIsOneStep) == input.iIsOneStepFilter);


            var query = (from o in filteredBuilds
                         join o1 in _databaseRepository.GetAll() on o.DatabaseId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetBuildForViewDto()
                         {
                             Build = ObjectMapper.Map<BuildDto>(o),
                             DatabasecDatabaseName = s1 == null ? "" : s1.cDatabaseName.ToString()
                         })
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DatabasecDatabaseNameFilter), e => e.DatabasecDatabaseName.ToLower() == input.DatabasecDatabaseNameFilter.ToLower().Trim());

            var totalCount = await query.CountAsync();

            var builds = await query
                .OrderBy(input.Sorting ?? "build.id asc")
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<GetBuildForViewDto>(
                totalCount,
                builds
            );
        }

        public async Task<GetBuildForViewDto> GetBuildForView(int id)
        {
            var build = await _buildRepository.GetAsync(id);

            var output = new GetBuildForViewDto { Build = ObjectMapper.Map<BuildDto>(build) };

            if (output.Build.DatabaseId != null)
            {
                var database = await _databaseRepository.FirstOrDefaultAsync((int)output.Build.DatabaseId);
                output.DatabasecDatabaseName = database.cDatabaseName.ToString();
            }

            return output;
        }
        public int GetLatestBuildFromDatabaseID(int databaseID)
        {
            if (_permissionChecker.IsGranted(_mySession.IDMSUserId, PermissionList.RunCountsInactiveBuild, AccessLevel.iAddEdit))
                return _buildRepository.Query(q => q.Where(p => p.iIsReadyToUse && p.DatabaseId.Equals(databaseID)).OrderByDescending(a => a.Id).Select(b => b.Id)).ToList().FirstOrDefault();
            else
                return _buildRepository.Query(q => q.Where(p => p.iIsOnDisk && p.iIsReadyToUse && p.DatabaseId.Equals(databaseID)).OrderByDescending(a => a.Id).Select(b => b.Id)).ToList().FirstOrDefault();
        }
        [AbpAuthorize(AppPermissions.Pages_Builds_Edit)]
        public async Task<GetBuildForEditOutput> GetBuildForEdit(EntityDto input)
        {
            var build = await _buildRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetBuildForEditOutput { Build = ObjectMapper.Map<CreateOrEditBuildDto>(build) };

            if (output.Build.DatabaseId != null)
            {
                var database = await _databaseRepository.FirstOrDefaultAsync((int)output.Build.DatabaseId);
                output.DatabasecDatabaseName = database.cDatabaseName.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditBuildDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Builds_Create)]
        private async Task Create(CreateOrEditBuildDto input)
        {
            var build = ObjectMapper.Map<Build>(input);
            await _buildRepository.InsertAsync(build);
        }

        [AbpAuthorize(AppPermissions.Pages_Builds_Edit)]
        private async Task Update(CreateOrEditBuildDto input)
        {
            var build = await _buildRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, build);
        }

        [AbpAuthorize(AppPermissions.Pages_Builds_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _buildRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_Builds)]
        public async Task<PagedResultDto<BuildDatabaseLookupTableDto>> GetAllDatabaseForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _databaseRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.cDatabaseName.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var databaseList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<BuildDatabaseLookupTableDto>();
            foreach (var database in databaseList)
            {
                lookupTableDtoList.Add(new BuildDatabaseLookupTableDto
                {
                    Id = database.Id,
                    DisplayName = database.cDatabaseName?.ToString()
                });
            }

            return new PagedResultDto<BuildDatabaseLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }


        public GetAllBuildsForDatabaseDto GetBuildsForDatabase(int iDatabaseID, int taskID)
        {
            try
            {
                var result = new GetAllBuildsForDatabaseDto();
                
                switch(taskID)
                {
                    case 8:
                    case 15:
                        result.BuildDropDown = (from build in _buildRepository.GetAll()
                                                join userDatabase in _userDatabaseRepository.GetAll()
                                                on build.DatabaseId equals userDatabase.DatabaseId
                                                where userDatabase.UserId == _mySession.IDMSUserId
                                                && build.DatabaseId == iDatabaseID
                                                orderby build.Id descending
                                                select new DropdownOutputDto
                                                {
                                                    Value = build.Id,
                                                    Label = $"{build.cDescription} : {build.Id}"
                                                }).ToList();
                        break;
                    case 17:
                        result.BuildDropDown = (from build in _buildRepository.GetAll()
                                                join userDatabase in _userDatabaseRepository.GetAll()
                                                on build.DatabaseId equals userDatabase.DatabaseId
                                                where build.iIsReadyToUse
                                                && !build.iIsOnDisk
                                                && userDatabase.UserId == _mySession.IDMSUserId
                                                && build.DatabaseId == iDatabaseID
                                                orderby build.Id descending
                                                select new DropdownOutputDto
                                                {
                                                    Value = build.Id,
                                                    Label = $"{build.cDescription} : {build.Id}"
                                                }).ToList();
                        break;
                    case 18:
                        result.BuildDropDown = (from build in _buildRepository.GetAll()
                                                join userDatabase in _userDatabaseRepository.GetAll()
                                                on build.DatabaseId equals userDatabase.DatabaseId
                                                where build.iIsOnDisk
                                                && userDatabase.UserId == _mySession.IDMSUserId
                                                && build.DatabaseId == iDatabaseID
                                                orderby build.cDescription ascending
                                                select new DropdownOutputDto
                                                {
                                                    Value = build.Id,
                                                    Label = $"{build.cDescription} : {build.Id}"
                                                }).ToList();
                        break;
                    default:
                        result.BuildDropDown = (from build in _buildRepository.GetAll()
                                                join userDatabase in _userDatabaseRepository.GetAll()
                                                on build.DatabaseId equals userDatabase.DatabaseId
                                                where build.iIsReadyToUse
                                                && userDatabase.UserId == _mySession.IDMSUserId
                                                && build.DatabaseId == iDatabaseID
                                                orderby build.Id descending
                                                select new DropdownOutputDto
                                                {
                                                    Value = build.Id,
                                                    Label = $"{build.cDescription} : {build.Id}"
                                                }).ToList();
                        break;
                }

                if (result.BuildDropDown.Count > 0)
                    result.DefaultSelection = Convert.ToInt32(result.BuildDropDown.FirstOrDefault().Value);

                return result;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public List<DropdownOutputDto> GetChildAndExternalTablesByBuild(int buildId, int databaseId)
        {
            try
            {
                var childTables = _buildTableRepository.GetAll().Where(bt => bt.BuildId == buildId && bt.LK_TableType == "C");
                var externalTables = _buildTableRepository.GetExternalTablesByDatabase(databaseId);
                var childTableDto = childTables.Select(bt => new DropdownOutputDto
                {
                    Label = bt.ctabledescription + " (" + bt.cTableName.Substring(0, bt.cTableName.IndexOf('_')) + ")",
                    Value = bt.Id
                }).ToList();
                if (externalTables != null && externalTables.Any())
                {
                    childTableDto.AddRange(externalTables.Select(et => new DropdownOutputDto
                    {
                        Label = et.ctabledescription + " (" + et.cTableName.Substring(0, et.cTableName.IndexOf('_')) + ")",
                        Value = et.Id
                    }));
                }
                return childTableDto;
            }
            catch (Exception)
            {
                throw new UserFriendlyException("Unable to load child tables");
            }
        }
    }
}