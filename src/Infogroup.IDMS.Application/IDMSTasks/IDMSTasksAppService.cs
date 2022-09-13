using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Domain.Repositories;
using Infogroup.IDMS.IDMSTasks.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Abp.UI;
using Infogroup.IDMS.BatchQueues;
using Infogroup.IDMS.Divisions;
using Infogroup.IDMS.Sessions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Infogroup.IDMS.IDMSConfigurations;
using System.Data;
using Infogroup.IDMS.ListLoadStatuses;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.SysSendMails;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.Campaigns.Dtos;
using Abp.Linq.Extensions;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.Shared.Dtos;
using Syncfusion.XlsIO;
using System.IO;
using Amazon;
using Amazon.Athena;
using Amazon.Athena.Model;
using Database = Infogroup.IDMS.Databases.Database;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.BuildTables.Dtos;

namespace Infogroup.IDMS.IDMSTasks
{
    [AbpAuthorize(AppPermissions.Pages_IDMSTasks)]
    public class IDMSTasksAppService : IDMSAppServiceBase, IIDMSTasksAppService
    {
        private readonly AppSession _mySession;
        private readonly IRepository<IDMSTask> _idmsTaskRepository;
        private readonly IIDMSTaskRepository _taskRepository;
        private readonly IRepository<BatchQueue> _batchQueueRepository;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IRepository<ListLoadStatus> _listLoadStatus;
        private readonly IRepository<Database> _databaseRepository;
        private readonly IRepository<SysSendMail> _sysSendMail;
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly ICampaignRepository _buildCampaignRepository;
        private readonly IBuildTableRepository _buildTableRepository;
        private readonly IIDMSPermissionChecker _permissionChecker;
        private const string LK_Status = "90";
        private const string Notes = "Changed by AOP from Previous Build task.";

        public IDMSTasksAppService(
              AppSession mySession,
              IRepository<IDMSTask> idmsTaskRepository,
              IRepository<BatchQueue> batchQueueRepository,
              IRedisIDMSConfigurationCache idmsConfigurationCache,
              IRepository<ListLoadStatus> listLoadStatus,
              IRepository<SysSendMail> sysSendMail,
              IRepository<Database> databaseRepository,
              IRepository<Campaign> campaignRepository,
              ICampaignRepository buildCampaignRepository,
              IIDMSTaskRepository taskRepository,
              IIDMSPermissionChecker permissionChecker,
              IBuildTableRepository buildTableRepository
            )
        {
            _mySession = mySession;
            _idmsTaskRepository = idmsTaskRepository;
            _batchQueueRepository = batchQueueRepository;
            _taskRepository = taskRepository;
            _listLoadStatus = listLoadStatus;
            _databaseRepository = databaseRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
            _campaignRepository = campaignRepository;
            _sysSendMail = sysSendMail;
            _buildCampaignRepository = buildCampaignRepository;
            _permissionChecker = permissionChecker;
            _buildTableRepository = buildTableRepository;
        }

        #region Fetch All Task
        public Task<PagedResultDto<GetIDMSTaskForViewDto>> GetAllTask(GetAllIDMSTasksInput input)
        {

            Dictionary<int, string> dictPermissions = new Dictionary<int, string>() {
                { 1, AppPermissions.Pages_IDMSTasks_SetValidEmailFlag },
                { 2, AppPermissions.Pages_IDMSTasks_ResendListtoAddressHygiene },
                { 3, AppPermissions.Pages_IDMSTasks_LoadMailerUsage },
                { 4, AppPermissions.Pages_IDMSTasks_ReportwithCustomColumns },
                { 5, AppPermissions.Pages_IDMSTasks_ExportListConversiondata },
                { 6, AppPermissions.Pages_IDMSTasks_ApogeeDataAppend },
                 { 7, AppPermissions.Pages_IDMSTasks_ApogeeExport },
                { 8, AppPermissions.Pages_IDMSTasks_SearchPreviousCampaignHistory },
                { 9, AppPermissions.Pages_IDMSTasks_CopyBuild },
                { 10, AppPermissions.Pages_IDMSTasks_LoadOptoutandHardBounce },
                { 11, AppPermissions.Pages_IDMSTasks_ExportEmailHygieneData },
                { 12, AppPermissions.Pages_IDMSTasks_ImportEmailHygieneData },
                 { 13, AppPermissions.Pages_IDMSTasks_ActivateLinkTable },
                { 14, AppPermissions.Pages_IDMSTasks_CloseNotificationJob },
                { 15, AppPermissions.Pages_IDMSTasks_DeleteBuildTask },
                { 16, AppPermissions.Pages_IDMSTasks_PenetrationReport },
                { 17, AppPermissions.Pages_IDMSTasks_BulkUpdateListAction },
                { 18, AppPermissions.Pages_IDMSTasks_ModelPivotReport}
            };

            var filteredTasks = _idmsTaskRepository.GetAll()
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Id.ToString().Equals(input.Filter) || e.cTaskDescription.Contains(input.Filter))
                                .Select(x =>
                                   new GetIDMSTaskForViewDto
                                   {
                                       ID = x.Id,
                                       cTaskDescription = x.cTaskDescription,
                                       cTaskName = x.cTaskName,
                                       cPermissionName = dictPermissions[x.Id]
                                   });
            var totalCount = filteredTasks.Count();

            var pagedAndFilteredTasks = filteredTasks
                .OrderBy(input.Sorting ?? "cTaskDescription asc").ToList();

            return Task.FromResult(new PagedResultDto<GetIDMSTaskForViewDto>(
                totalCount,
                pagedAndFilteredTasks
            ));
        }
        #endregion

        #region Set Valid Email Flag
        public void SetValidEmailFlag(SetValidEmailFlagDto input)
        {
            try
            {
                validateDateTime(input, out DateTime? scheduledDateTime);

                var queue = new BatchQueue
                {
                    ListId = input.TaskGeneralTo.DatabaseID,
                    BuildId = input.TaskGeneralTo.BuildID,
                    DivisionId = _taskRepository.GetDivionIdFromDatabase(input.TaskGeneralTo.DatabaseID),
                    ProcessTypeId = BatchProcessType.SetValidEmailFlag.GetHashCode(),
                    ParmData = input.TaskGeneralFrom.BuildID.ToString(),
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                    iPriority = 3,
                    cScheduledBy = _mySession.IDMSUserFullName,
                    IsStopRequest = false,
                    Recipients = _mySession.IDMSUserEmail,
                    dRecordDate = DateTime.Now,
                    dScheduled = scheduledDateTime
                };

                _batchQueueRepository.Insert(queue);

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private void validateDateTime(SetValidEmailFlagDto Input, out DateTime? scheduleDateTime)
        {
            scheduleDateTime = null;
            var date = DateTime.Now.Date;
            var time = DateTime.Now.TimeOfDay;

            if (Input.TaskGeneralFrom.BuildID == Input.TaskGeneralTo.BuildID)
            {
                throw new UserFriendlyException(L("SameBuildError"));
            }
            if (!DateTime.TryParse(Input.ScheduledDate, out date))
            {
                throw new UserFriendlyException(L("dateError"));
            }
            else if (!TimeSpan.TryParse(Input.ScheduledTime, out time))
            {
                throw new UserFriendlyException(L("timeError"));
            }
            else
            {
                date = date.Add(time);
                if (date.CompareTo(DateTime.Now).Equals(0) || date.CompareTo(DateTime.Now).Equals(-1))
                {
                    throw new UserFriendlyException(L("dateTimeError"));
                }
                else
                {
                    scheduleDateTime = date;
                }
            }
        }
        #endregion

        #region AOP From previous build 
        public bool AOPFrompreviousbuild(AOPFromPreviousBuildDto input)
        {
            var aws = _idmsConfigurationCache.GetConfigurationValue("AWS", input.TaskGeneral.DatabaseID).cValue;
            if (aws == "1")
            {
                bool tableCreated = CreateAthenaTable(input);
                return true;
            }
            else
            {
                try
                {
                    var errors = new StringBuilder();
                    var sbTables = new StringBuilder();
                    var sbListID = new StringBuilder();
                    var ListIds = new List<string>();
                    var sb = new StringBuilder();
                    var RemovedTables = new StringBuilder();
                    var RenameDone = false;


                    var connectionString = _idmsConfigurationCache.GetConfigurationValue("ConnectionString2", input.TaskGeneral.DatabaseID).cValue;
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        var listIds = input.ListId.Trim().Split(new char[] { ',' });
                        foreach (var tempId in listIds)
                        {
                            if (!string.IsNullOrEmpty(tempId.Trim()))
                            {
                                sbTables.AppendFormat("'DW_Final_{0}_{1}_{2}',", input.TaskGeneral.DatabaseID, input.NewBuildID, tempId.Trim());
                                sb.AppendFormat("'DW_Final_{0}_{1}_{2}',", input.TaskGeneral.DatabaseID, input.TaskGeneral.BuildID, tempId.Trim());
                                sbListID.Append(tempId.Trim() + ",");
                                ListIds.Add(tempId.Trim());
                            }
                        }
                        if (sbTables.Length > 0)
                        {
                            var result = "";
                            sbTables.Length -= 1;
                            sb.Length -= 1;
                            var objT = new IDMSTask();

                            var dataSet = _taskRepository.CheckTableName(connectionString, sbTables.ToString());

                            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                            {
                                var builder = new StringBuilder();
                                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                                {
                                    builder.Append(dataRow["ListID"] + ",");
                                    RemovedTables.AppendFormat("{0},", dataRow["name"]);
                                }
                                result = builder.ToString();
                                result = result.Remove(result.LastIndexOf(","));
                                RemovedTables.Length -= 1;
                            }
                            if (!string.IsNullOrEmpty(result))
                            {
                                if (!input.isRegister || !input.userAgree)
                                {
                                    input.isRegister = true;
                                    RenameDone = false;
                                }
                                else if (input.userAgree)
                                {
                                    input.userAgree = false;
                                    var lstID = result.Split(',');
                                    _taskRepository.DropTableTORename(connectionString, sbTables.ToString());
                                    RenameTable(listIds, connectionString, input.TaskGeneral.DatabaseID, input.TaskGeneral.BuildID, input.NewBuildID, errors);
                                    SendConfirmationMail(RemovedTables.ToString(), input.TaskGeneral.DatabaseID);
                                    RenameDone = true;
                                }
                            }
                            else
                            {
                                RenameTable(listIds, connectionString, input.TaskGeneral.DatabaseID, input.TaskGeneral.BuildID, input.NewBuildID, errors);
                                RenameDone = true;
                            }
                        }
                        else
                        {
                            RenameTable(listIds, connectionString, input.TaskGeneral.DatabaseID, input.TaskGeneral.BuildID, input.NewBuildID, errors);
                            RenameDone = true;
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException(L("connectionStringError"));
                    }

                    return RenameDone;
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }
            }
        }

        private void SendConfirmationMail(string TableNames, int databaseID)
        {
            var objE = _databaseRepository.GetAll().FirstOrDefault(x => x.Id == databaseID);

            var sbBody = new StringBuilder();
            sbBody.Append(L("DroppedTable")).Append(Environment.NewLine);
            sbBody.Append(L("ContactAdminForReloadFile")).Append(Environment.NewLine);
            sbBody.Append(Environment.NewLine);

            sbBody.Append(L("DatabaseForEmail", objE.cDatabaseName.ToString())).Append(Environment.NewLine);
            sbBody.Append(L("TableNameForEmail", TableNames)).Append(Environment.NewLine);
            sbBody.Append(L("UserIdForEmail", _mySession.IDMSUserId)).Append(Environment.NewLine);
            sbBody.Append(L("DateTimeForEmail", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"))).Append(Environment.NewLine);
            sbBody.Append(L("IDMSAdminForEmail", objE.cAdministratorEmail)).Append(Environment.NewLine);


            var objMail = new SysSendMail
            {
                cSubject = L("EmailSubject"),
                cMessage = sbBody.ToString(),
                cRecipients = _mySession.IDMSUserEmail,
                cCopyRecipients = $@"IDMSSystemNotification@infogroup.com; {objE.cAdministratorEmail}"
            };


            _sysSendMail.Insert(objMail);

            CurrentUnitOfWork.SaveChanges();


        }

        private void RenameTable(string[] listIds, string connection, int databaseId, int fromBuildId, int toBuildId, StringBuilder errors)
        {
            foreach (var tempId in listIds)
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempId.Trim()))
                    {
                        var Listid = Convert.ToInt32(tempId.Trim());

                        var oldTableName = $@"DW_Final_{databaseId}_{fromBuildId}_{Listid}";
                        var newTableName = $@"DW_Final_{databaseId}_{toBuildId}_{Listid}";

                        var BuildLolId = _taskRepository.GetBuildLolID(toBuildId, Listid.ToString());

                        var objLoadStatus = _listLoadStatus.GetAll().FirstOrDefault(x => x.BuildLoLID == BuildLolId && x.iIsCurrent);

                        _taskRepository.RenameTables(connection, oldTableName, newTableName);

                        if (objLoadStatus.LK_LoadStatus.Trim() != LK_Status)
                        {

                            var ListEntity = new ListLoadStatus
                            {
                                BuildLoLID = objLoadStatus.BuildLoLID,
                                cCalculation = string.Empty,
                                iIsCurrent = true,
                                LK_LoadStatus = LK_Status,
                                cNotes = Notes,
                                cCreatedBy = _mySession.IDMSUserName,
                                dCreatedDate = DateTime.Now
                            };
                            _listLoadStatus.InsertAsync(ListEntity);
                            CurrentUnitOfWork.SaveChanges();
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }
            }
        }

        #endregion

        #region Load Mailer Usage 
        public void LoadMailerUsage(LoadMailerUsageDto input)
        {
            try
            {
                _taskRepository.TaskLoadMailerUsuage(input);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Enhanced Audit Report 
        public void EnhancedAuditReport(EnhancedAuditReportDto input)
        {
            try
            {
                ValidateScheduledDateTime(input.ScheduledDate, input.ScheduledTime, out DateTime? scheduledDateTime);
                var campaign = new Campaign();
                var CountID = string.IsNullOrEmpty(input.CountId)? 0 : Convert.ToInt32(input.CountId);
                if (!string.IsNullOrEmpty(input.CountId))
                {
                    campaign = _campaignRepository.GetAll().FirstOrDefault(x => x.Id == CountID);
                    if (campaign == null)
                    {
                        throw new UserFriendlyException(L("InvalidCountID"));
                    }
                }

                var cValue = string.Empty;
                var build = new BuildDetails();
                if (CountID != 0)
                {
                    build = _buildCampaignRepository.GetBuildDetails(CountID);
                    if (build != null)
                    {
                        cValue = _idmsConfigurationCache.GetConfigurationValue("AuditKey", build.DatabaseID).cValue;
                    }
                }
                else
                {
                    cValue = _idmsConfigurationCache.GetConfigurationValue("AuditKey", input.TaskGeneral.DatabaseID).cValue;
                }

                if (!string.IsNullOrEmpty(cValue))
                {

                    var oBatchQueue = new BatchQueue
                    {
                        DivisionId = (CountID == 0) ? _taskRepository.GetDivionIdFromDatabase(input.TaskGeneral.DatabaseID) : build.DivisionID,
                        ProcessTypeId = BatchProcessType.AuditReport.GetHashCode(),
                        BuildId = (CountID == 0) ? input.TaskGeneral.BuildID : campaign.BuildID,
                        FieldName = cValue,
                        Recipients = _mySession.IDMSUserEmail,
                        dRecordDate = DateTime.Now,
                        dScheduled = scheduledDateTime,
                        cScheduledBy = _mySession.IDMSUserFullName,
                        iPriority = 3,
                        IsStopRequest = false,
                        iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                        ParmData = input.CountId,
                        DayDiff = input.IsListWiseReport ? 1 : 0
                    };

                    _batchQueueRepository.Insert(oBatchQueue);

                    CurrentUnitOfWork.SaveChanges();
                }
                else
                {
                    throw new UserFriendlyException(L("noRecordForAuditReport"));
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Export List Conversion Data 
        public int ExportListConversionData(ExportListConversionDataDto input)
        {
            try
            {
                var queueID = 0;
                if (!string.IsNullOrEmpty(input.ListId))
                {
                    if (_taskRepository.GetBuildLolID(input.TaskGeneral.BuildID, input.ListId.Trim()) == 0)
                    {
                        throw new UserFriendlyException(L("ListidNotPresent", input.ListId.Trim()));
                    }

                    var configPath = _idmsConfigurationCache.GetConfigurationValue("ExportListTaskPath", input.TaskGeneral.DatabaseID);
                    if (configPath == null)
                    {
                        throw new UserFriendlyException(L("PathNotFoundForDatabase"));
                    }
                    else
                    {
                        if (!configPath.cValue.EndsWith(@"\"))
                            configPath.cValue += @"\";

                        var ext = "txt";

                        if (input.OutputType == "EE")
                            ext = "xlsx";

                        var FileName = $@"{configPath.cValue}ExportListData_{input.TaskGeneral.BuildID}_{input.ListId.Trim()}_{DateTime.Now:MM-dd-yy}_{DateTime.Now:HH-mm-ss}.{ext}";
                        if (FileName.Length > 255)
                        {
                            throw new UserFriendlyException(L("LongFilePath"));
                        }

                        var BatchQueue = new BatchQueue
                        {
                            DivisionId = _taskRepository.GetDivionIdFromDatabase(input.TaskGeneral.DatabaseID),
                            ProcessTypeId = BatchProcessType.ExportListConversion.GetHashCode(),
                            ListId = Convert.ToInt32(input.ListId.Trim()),
                            BuildId = input.TaskGeneral.BuildID,
                            FieldName = input.OutputType,
                            JoinKeyId = input.OutputQuantity,
                            FileName = FileName,
                            Recipients = _mySession.IDMSUserEmail,
                            dRecordDate = DateTime.Now,
                            dScheduled = DateTime.Now,
                            cScheduledBy = _mySession.IDMSUserFullName,
                            iPriority = 3,
                            IsStopRequest = false,
                            iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                            ParmData = $@"{input.Fields}:|:{input.Selection}",
                            DayDiff = input.TableType
                        };

                        queueID = _batchQueueRepository.InsertAndGetId(BatchQueue);

                        CurrentUnitOfWork.SaveChanges();                        
                    }                    
                }
                return queueID;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Apogee Custom Export Task
        public void ApogeeCustomExportTask(ApogeeCustomExportTaskDto input)
        {
            try
            {
                ValidateScheduledDateTime(input.ScheduledDate, input.ScheduledTime, out DateTime? scheduledDateTime);
                var CountID = Convert.ToInt32(input.CountId);
                if (!string.IsNullOrEmpty(input.CountId.Trim()))
                {
                    var campaign = _campaignRepository.GetAll().FirstOrDefault(x => x.Id == CountID);
                    if (campaign == null)
                    {
                        throw new UserFriendlyException(L("InvalidCountID"));
                    }
                }
                if (CountID != 0)
                {
                    var build = _buildCampaignRepository.GetBuildDetails(CountID);
                    if (build != null)
                    {
                        var queue = new BatchQueue
                        {
                            DivisionId = build.DivisionID,
                            BuildId = build.Id,
                            ParmData = build.BuildID.ToString(),
                            DayDiff = build.DatabaseID,
                            dScheduled = scheduledDateTime,
                            ListId = CountID,
                            ProcessTypeId = BatchProcessType.ApogeeCustomTask.GetHashCode(),
                            iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                            cScheduledBy = _mySession.IDMSUserFullName,
                            IsStopRequest = false,
                            Recipients = _mySession.IDMSUserEmail.Trim(),
                            dRecordDate = DateTime.Now,
                            iPriority = 3
                        };

                        _batchQueueRepository.Insert(queue);

                        CurrentUnitOfWork.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Apogee Export Point-In-Time Task 
        public void ApogeeExportPointInTimeTask(ApogeeExportPointInTimeTaskDto input)
        {
            try
            {
                var dsCurrent = _taskRepository.GetDivisionIdFromDatabaseAndBuild(input.TaskGeneral.BuildID, input.TaskGeneral.DatabaseID);
                if (dsCurrent != null)
                {
                    var queue = new BatchQueue
                    {
                        DivisionId = dsCurrent.DivisionID,
                        DayDiff = dsCurrent.BuildID,
                        BuildId = dsCurrent.Id,
                        ParmData = $@"{input.FileLocation}{input.InputFileName}",
                        ListId = 82,
                        ProcessTypeId = BatchProcessType.ApogeeExportPointInTimeData.GetHashCode(),
                        iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                        cScheduledBy = _mySession.IDMSUserFullName,
                        IsStopRequest = false,
                        Recipients = _mySession.IDMSUserEmail.Trim(),
                        dRecordDate = DateTime.Now,
                        iPriority = 3,
                        dScheduled = DateTime.Now,
                    };

                    _batchQueueRepository.Insert(queue);

                    CurrentUnitOfWork.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public string getFilePathOfApogeePIT()
        {
            return _idmsConfigurationCache.GetConfigurationValue("ApogeePITDefaultPath").cValue;
        }
        #endregion

        #region Search Previous Order History by Key Task
        public void SearchPreviousOrderHistorybyKeyTask(SearchPreviousOrderHistorybyKeyTaskDto input)
        {
            try
            {
                if (input.StartDate != DateTime.MinValue && input.EndDate != DateTime.MinValue)
                {
                    if (DateTime.Compare(input.StartDate.Date, input.EndDate.Date) > 0)
                    {
                        throw new UserFriendlyException(L("StartDateCannotBeGreater"));
                    }
                }
                var Database = _databaseRepository.GetAll().FirstOrDefault(x => x.Id == input.TaskGeneral.DatabaseID);

                var StartDate = input.StartDate != DateTime.MinValue ? input.StartDate : Convert.ToDateTime("1900-01-01");
                var EndDate = input.EndDate != DateTime.MinValue ? input.EndDate : DateTime.Today;
                var queue = new BatchQueue
                {
                    DivisionId = Database.DivisionId,
                    BuildId = input.TaskGeneral.BuildID,
                    ListId = input.TaskGeneral.DatabaseID,
                    JoinKeyName = input.SearchKey.Trim(),
                    FileName = Database.cDatabaseName,
                    ParmData = StartDate.ToString("yyyyMMdd") + "|" + EndDate.ToString("yyyyMMdd"),
                    FieldName = _taskRepository.GetFieldName(input.TaskGeneral.BuildID),
                    ProcessTypeId = BatchProcessType.SearchOrderHistorybyKey.GetHashCode(),
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                    cScheduledBy = _mySession.IDMSUserFullName,
                    IsStopRequest = false,
                    DayDiff = input.CampaignId != 0 ? input.CampaignId : 0,
                    
                    Recipients = _mySession.IDMSUserEmail.Trim(),
                    dRecordDate = DateTime.Now,
                    iPriority = 3,
                    dScheduled = DateTime.Now
                };

                _batchQueueRepository.Insert(queue);

                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Copy Build Task
        public int CopyBuild(CopyBuildDto input)
        {
            try
            {
                var ErrorMessage = string.Empty;
                var Build = _taskRepository.ValidateCopyBuild(input.TargetBuild, input.SourceBuild);
                if (Build.DatabaseId == 0)
                    ErrorMessage += L("BuildShouldFromSameDatabase");
                else if (Build.iIsReadyToUse)
                    ErrorMessage += L("BuildAlreadyCopied");
                else if (!_permissionChecker.IsGranted(_mySession.IDMSUserId, PermissionList.Build, AccessLevel.iAddEdit))
                    ErrorMessage += L("NotPermitoCreateBuild");

                if (!string.IsNullOrEmpty(ErrorMessage))
                    throw new UserFriendlyException(ErrorMessage);

                var queue = new BatchQueue
                {
                    JoinKeyId = Build.DatabaseId,
                    BuildId = input.TargetBuild,
                    DivisionId = _taskRepository.GetDivionIdFromDatabase(Build.DatabaseId),
                    ProcessTypeId = BatchProcessType.CopyBuildTask.GetHashCode(),
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                    iPriority = 3,
                    cScheduledBy = _mySession.IDMSUserFullName,
                    Recipients = _mySession.IDMSUserEmail,
                    dRecordDate = DateTime.Now,
                    dScheduled = DateTime.Now,
                    dStartDate = DateTime.Now,
                    ParmData = input.SourceBuild.ToString()
                };

                var NewInsertedID = _batchQueueRepository.InsertAndGetId(queue);

                CurrentUnitOfWork.SaveChanges();
                return NewInsertedID;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Optout Hardbounce Task
        public int OptoutHardbounceTask(OptoutHardbounceTaskDto input)
        {
            try
            {
                var queue = new BatchQueue
                {
                    DivisionId = _taskRepository.GetDivionIdFromDatabase(input.TaskGeneral.DatabaseID),
                    ParmData = input.FileType.Trim(),
                    FileName = input.InputFileName.Trim(),
                    FolderName = input.FileLocation.Trim(),
                    ProcessTypeId = BatchProcessType.OptOutAndHardBounceFile.GetHashCode(),
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                    ListId = null,
                    BuildId = input.TaskGeneral.BuildID,
                    JoinKeyId = input.TaskGeneral.DatabaseID,
                    cScheduledBy = _mySession.IDMSUserFullName,
                    IsStopRequest = false,
                    Recipients = _mySession.IDMSUserEmail.Trim(),
                    dRecordDate = DateTime.Now,
                    iPriority = 3,
                    dScheduled = DateTime.Now
                };

                var queueID = _batchQueueRepository.InsertAndGetId(queue);

                CurrentUnitOfWork.SaveChanges();

                return queueID;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public string getFilePathOfOptoutHardbounce()
        {
            try
            {
                return _idmsConfigurationCache.GetConfigurationValue("Optout&HardBoundPath").cValue;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Export Email Hygiene Data
        public int ExportEmailHygieneData(ExportEmailHygieneDataDto input)
        {
            try
            {
                var queue = new BatchQueue
                {
                    DivisionId = _taskRepository.GetDivionIdFromDatabase(input.TaskGeneral.DatabaseID),
                    ListId = input.TaskGeneral.DatabaseID,
                    BuildId = input.TaskGeneral.BuildID,
                    Recipients = _mySession.IDMSUserEmail,
                    ParmData = $@"ExportField:{input.ExportField.Trim()}|FlagField:{input.ExportFlagField.Trim()}|FlagValues:{input.FlagValue.Trim()}",
                    ProcessTypeId = BatchProcessType.ExportEmailHygiene.GetHashCode(),
                    dRecordDate = DateTime.Now,
                    dScheduled = DateTime.Now,
                    cScheduledBy = _mySession.IDMSUserFullName,
                    iPriority = 2,
                    IsStopRequest = false,
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                    FileName = GetEntityByCItemOverrideDB(input.TaskGeneral.DatabaseID)
                };

                var insertedQueue = _batchQueueRepository.InsertAndGetId(queue);

                CurrentUnitOfWork.SaveChanges();

                return insertedQueue;
                
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        private string GetEntityByCItemOverrideDB(int databaseID)
        {
            try
            {
                return _idmsConfigurationCache.GetConfigurationValue("EMAILHYGIENEEXPORTPATH", databaseID).cValue;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Import Email Hygiene Data
        public int ImportEmailHygieneData(ImportEmailHygieneDataDto input)
        {
            try
            {
                var queue = new BatchQueue
                {
                    DivisionId = _taskRepository.GetDivionIdFromDatabase(input.TaskGeneral.DatabaseID),
                    ListId = input.TaskGeneral.DatabaseID,
                    BuildId = input.TaskGeneral.BuildID,
                    Recipients = _mySession.IDMSUserEmail,
                    ParmData = $@"MatchField:{input.MatchField}|FlagField:{input.FlagField}|FilePath:{input.FilePath}",
                    ProcessTypeId = BatchProcessType.ImportEmailHygiene.GetHashCode(),
                    dRecordDate = DateTime.Now,
                    dScheduled = DateTime.Now,
                    cScheduledBy = _mySession.IDMSUserFullName,
                    iPriority = 2,
                    IsStopRequest = false,
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                    FileName = input.FileName.Trim()
                };

                var insertedQueue = _batchQueueRepository.InsertAndGetId(queue);

                CurrentUnitOfWork.SaveChanges();

                return insertedQueue;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public string GetImportEmailHygieneLoadPath(int databaseID)
        {
            try
            {
                return _idmsConfigurationCache.GetConfigurationValue("EMAILHYGIENEIMPORTPATH", databaseID).cValue;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region Activate Link Table Build
        public int ActivateLinkTableBuild(ActivateLinkTableBuildDto input)
        {
            try
            {
                ValidateScheduledDateTime(input.ScheduledDate,input.ScheduledTime, out DateTime? scheduledDateTime);
                var BuildID = _idmsConfigurationCache.GetConfigurationValue("ExternalTableBuildID").cValue;
                var DatabaseID = _taskRepository.GetDataSetDatabaseByBuildID(Convert.ToInt32(BuildID));
                var queue = new BatchQueue
                {
                    DivisionId = _taskRepository.GetDivionIdFromDatabase(DatabaseID),
                    ListId = DatabaseID,//Adding DatabaseID
                    BuildId = Convert.ToInt32(BuildID),
                    Recipients = _mySession.IDMSUserEmail,
                    dScheduled = scheduledDateTime,
                    ProcessTypeId = BatchProcessType.ActivateLinkTable.GetHashCode(),
                    dRecordDate = DateTime.Now,
                    cScheduledBy = _mySession.IDMSUserFullName,
                    iPriority = 2,
                    IsStopRequest = false,
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                    ParmData = $@"TableName:{input.TableName}"
                };

                var insertedQueueID = _batchQueueRepository.InsertAndGetId(queue);

                CurrentUnitOfWork.SaveChanges();

                return insertedQueueID;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }        

        public GetAllTableDescriptionFromBuild GetAllTableDescriptionForBuild()
        {
            try
            {
                var BuildID = _idmsConfigurationCache.GetConfigurationValue("ExternalTableBuildID");
                var result = new GetAllTableDescriptionFromBuild
                {
                    ExportFlagFieldDropdown = _taskRepository.GetAllTableDescription(BuildID.cValue),
                    DefaultSelection = _taskRepository.GetAllTableDescription(BuildID.cValue).FirstOrDefault().Value.ToString(),
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Close Notification Job
        public int CloseNotificationJob(TaskGeneralDto input)
        {
            try
            {
                var queue = new BatchQueue
                {
                    DivisionId = _taskRepository.GetDivionIdFromDatabase(input.DatabaseID),
                    ListId = input.DatabaseID,
                    BuildId = input.BuildID,
                    Recipients = _mySession.IDMSUserEmail,
                    ProcessTypeId = BatchProcessType.CloseNotificationJob.GetHashCode(),
                    dRecordDate = DateTime.Now,
                    dScheduled = DateTime.Now,
                    cScheduledBy = _mySession.IDMSUserFullName,
                    iPriority = 2,
                    IsStopRequest = false,
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode()
                };

                var InsertedRowID = _batchQueueRepository.InsertAndGetId(queue);

                CurrentUnitOfWork.SaveChanges();

                return InsertedRowID;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Delete Build Task
        public int BuildDeleteTask(TaskGeneralDto input)
        {
            try
            {
                var queue = new BatchQueue
                {
                    DivisionId = _taskRepository.GetDivionIdFromDatabase(input.DatabaseID),
                    ListId = input.DatabaseID,
                    BuildId = input.BuildID,
                    Recipients = _mySession.IDMSUserEmail,
                    ProcessTypeId = BatchProcessType.DeleteBuild.GetHashCode(),
                    dRecordDate = DateTime.Now,
                    dScheduled = DateTime.Now,
                    cScheduledBy = _mySession.IDMSUserFullName,
                    iPriority = 2,
                    IsStopRequest = false,
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode()
                };

                var insertedQueueID = _batchQueueRepository.InsertAndGetId(queue);

                CurrentUnitOfWork.SaveChanges();

                return insertedQueueID;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Bulk Update List Action
        public int BulkUpdateListAction(BulkUpdateListActionDto input)
        {
            try
            {
                var timeUtc = DateTime.UtcNow;
                var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
                var insertedQueueID = 0;

                if (IsValidFile(input))
                {
                    var queue = new BatchQueue
                    {
                        DivisionId = _taskRepository.GetDivionIdFromDatabase(input.TaskGeneral.DatabaseID),
                        ListId = input.TaskGeneral.DatabaseID,
                        BuildId = input.TaskGeneral.BuildID,
                        Recipients = _mySession.IDMSUserEmail,
                        ProcessTypeId = BatchProcessType.BulkUploadListAction.GetHashCode(),
                        dRecordDate = easternTime,
                        dScheduled = easternTime,
                        cScheduledBy = _mySession.IDMSUserFullName,
                        iPriority = 2,
                        IsStopRequest = false,
                        FileName = input.FileName,
                        iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                        ParmData = _mySession.IDMSUserName
                    };

                    insertedQueueID = _batchQueueRepository.InsertAndGetId(queue);

                    CurrentUnitOfWork.SaveChanges();

                    return insertedQueueID;
                }

                return insertedQueueID;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private bool IsValidFile(BulkUpdateListActionDto input)
        {
            var result = true;
            
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    var application = excelEngine.Excel;
                    var inputStream = new FileStream(input.FileName, FileMode.Open);
                    var workBook = application.Workbooks.Open(inputStream);
                    var sheet = workBook.Worksheets[0];
                    sheet.UsedRangeIncludesFormatting = false;
                    var dataTable = sheet.ExportDataTable(sheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                    inputStream.Close();
                    inputStream.Dispose();
                    //Ignoring rows where empty cells contains null or empty value
                    var newDataTable = (from row in dataTable.AsEnumerable()
                                        where !string.IsNullOrEmpty(row.Field<string>(dataTable.Columns[0].ColumnName)) && !string.IsNullOrEmpty(row.Field<string>(dataTable.Columns[1].ColumnName))
                                        select row).CopyToDataTable();

                    result = ValidateFile(newDataTable, input.TaskGeneral.BuildID, out StringBuilder ErrorMessage);
                    if(!result)
                    {
                        throw new UserFriendlyException(ErrorMessage.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            return result;
        }

        private bool ValidateFile(DataTable dt, int BuildID, out StringBuilder ErrorMessage)
        {
            ErrorMessage = new StringBuilder();
            if (dt.Columns.Count > 2 || dt.Columns.Count < 2)
            {
                ErrorMessage.Append(L("FileContentLessThanAndGreaterThanTwo"));
                return false;
            }
            //Duplicate ListIds in the List
            var duplicateListIds = dt.AsEnumerable()
                           .GroupBy(x => new
                           {
                               listIds = x.Field<string>(dt.Columns[0].ColumnName)
                           }).Where(gr => gr.Count() > 1)
                        .Select(g => g.Key).ToArray().ToList();



            if (duplicateListIds.Count() > 0)
            {
                ErrorMessage.Append(L("DuplicateListID", string.Join(",", duplicateListIds.Select(t => t.listIds).ToArray())));
                return false;
            }
            //If listids not found in tblBuildLOL
            var masterListIds = _taskRepository.GetAllListForBuild(BuildID).Select(t => t.MasterLolID).ToList();
            var excelListIds = dt.AsEnumerable().Select(t => t.Field<string>(dt.Columns[0].ColumnName)).ToList().Select(int.Parse).ToList();
            var IsNotInBuildLolList = excelListIds.Except(masterListIds).ToList();
            if (IsNotInBuildLolList.Count > 0)
            {

                ErrorMessage.Append(L("ListIdNotFound",string.Join(",", IsNotInBuildLolList.Select(t => t.ToString()).ToArray())));
                return false;
            }
            return true;
        }
        #endregion

        #region Common methods for fetching the import/export email hygiene data
        public GetAllExportFlagFieldForBuildDto GetAllExportFlagFieldForBuild(int BuildID)
        {
            try
            {
                var result = new GetAllExportFlagFieldForBuildDto
                {
                    ExportFlagFieldDropdown = _taskRepository.GetExportFlagFields(BuildID),
                    DefaultSelection = _taskRepository.GetExportFlagFields(BuildID).FirstOrDefault().Value.ToString(),
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Commom validation methods
        private void ValidateScheduledDateTime(string inputDate, string inputTime, out DateTime? scheduleDateTime)
        {
            scheduleDateTime = null;
            var date = DateTime.Now.Date;
            var time = DateTime.Now.TimeOfDay;

            if (!DateTime.TryParse(inputDate, out date))
            {
                throw new UserFriendlyException(L("dateError"));
            }
            else if (!TimeSpan.TryParse(inputTime, out time))
            {
                throw new UserFriendlyException(L("timeError"));
            }
            else
            {
                date = date.Add(time);
                if (date.CompareTo(DateTime.Now).Equals(0) || date.CompareTo(DateTime.Now).Equals(-1))
                {
                    throw new UserFriendlyException(L("dateTimeError"));
                }
                else
                {
                    scheduleDateTime = date;
                }
            }
        }

        public GetServerDateTime GetServerDate()
        {
            var serverDateTime = new GetServerDateTime
            {
                Date = DateTime.Now.Date.ToShortDateString(),
                Time = DateTime.Now.AddMinutes(1).TimeOfDay.ToString("hh\\:mm")
            };

            return serverDateTime;
        }
        #endregion

        #region Code for AWS Athena creation of tables
        private bool CreateAthenaTable(AOPFromPreviousBuildDto input)
        {
            AthenaAPIClient athenaAPIClient = new AthenaAPIClient();
            string sql = $"DROP TABLE IF EXISTS dw_final_{input.TaskGeneral.DatabaseID}_{input.NewBuildID}_{input.ListId}";
            string createSQlStr = $"CREATE TABLE dw_final_{input.TaskGeneral.DatabaseID}_{input.NewBuildID}_{input.ListId} AS SELECT * FROM dw_final_{input.TaskGeneral.DatabaseID}_{input.TaskGeneral.BuildID}_{input.ListId}";
            //we need to add table from table standered
            athenaAPIClient.ExecuteNonQuery(sql); //which SQL statement need to execute here
            athenaAPIClient.ExecuteNonQuery(createSQlStr); //which SQL statement need to execute here
            return true;
        }
        #endregion

        #region Model Pivot Report
        public async Task<int> SaveModelPivotReport(ModelPivotReportDto input)
        {
            try
            {
                var childTables = _buildTableRepository.GetAll()
                                    .Where(bt => bt.BuildId == input.BuildID && bt.LK_TableType == "C")
                                    .Select(item => new BuildTableDto
                                    {
                                        Id = item.Id,
                                        cTableName = item.cTableName,
                                        ctabledescription = item.ctabledescription
                                    }).ToList();

                var externalTables = _buildTableRepository.GetExternalTablesByDatabase(input.DatabaseID);
                if (externalTables != null && externalTables.Count > 0)
                {
                    childTables.AddRange(externalTables);
                }

                var model1 = childTables.FirstOrDefault(ct => ct.Id == Convert.ToInt32(input.Model1));
                var model2 = childTables.FirstOrDefault(ct => ct.Id == Convert.ToInt32(input.Model2));

                var parmData = $@"{model1.cTableName},{model2.cTableName}|{model1.ctabledescription},{model2.ctabledescription}";

                var queue = new BatchQueue
                {
                    ListId = input.DatabaseID,
                    BuildId = input.BuildID,
                    DivisionId = _taskRepository.GetDivionIdFromDatabase(input.DatabaseID),
                    ProcessTypeId = BatchProcessType.ModelCrossTabReport.GetHashCode(),
                    iStatusId = BatchQueueStatus.Waiting.GetHashCode(),
                    iPriority = 2,
                    cScheduledBy = _mySession.IDMSUserName,
                    IsStopRequest = false,
                    cStopRequestedBy = "",
                    DayDiff = 0,
                    JoinKeyId = 0,
                    JoinKeyName = "",
                    ResponseLength = 0,
                    FillerLength = 0,
                    FolderName = "",
                    FileName = "",
                    FieldName = "",
                    Recipients = _mySession.IDMSUserEmail,
                    dRecordDate = DateTime.Now,
                    dScheduled = DateTime.Now,
                    dStartDate = null,
                    dEndDate = null,
                    dStopped = null,
                    ParmData = parmData,
                    Result = ""
                };

                var insertedRowID = _batchQueueRepository.InsertAndGetId(queue);

                await CurrentUnitOfWork.SaveChangesAsync();

                return insertedRowID;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }
}