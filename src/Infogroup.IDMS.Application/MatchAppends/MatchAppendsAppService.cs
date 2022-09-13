

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.BatchQueues;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.MatchAppendInputLayouts;
using Infogroup.IDMS.MatchAppendOutputLayouts;
using Infogroup.IDMS.MatchAppends.Dtos;
using Infogroup.IDMS.MatchAppendStatuses;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.ShortSearch;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infogroup.IDMS.MatchAppends
{
    [AbpAuthorize(AppPermissions.Pages_MatchAppends)]
    public class MatchAppendsAppService : IDMSAppServiceBase, IMatchAppendsAppService
    {
        private const string _s3PathRegex = @"[s|S]3:\/\/(?<bucket>[^\/]+)\/(?<key>.+)";
        private readonly IMatchAppendsRepository _customMatchAppendRepository;
        private readonly IRepository<MatchAppendInputLayout, int> _matchAppendInputLayoutRepository;
        private readonly IRepository<MatchAppendStatus, int> _matchAppendStatusRepository;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IRepository<BatchQueue, int> _batchQueueRepository;
        private readonly IRepository<Build, int> _buildRepository;
        private readonly IBuildTableRepository _buildTableRepository;
        private readonly IShortSearch _shortSearch;
        private readonly AppSession _mySession;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IRepository<Lookup, int> _lookupRepository;
        private readonly IRepository<MatchAppendOutputLayout, int> _matchAppendOutputLayoutRepository;
        private readonly IBuildTableLayoutRepository _customBuildTableLayoutRepository;



        public MatchAppendsAppService(
             AppSession mySession,
             IMatchAppendsRepository customMatchAppendRepository,
             IRepository<MatchAppendInputLayout, int> matchAppendInputLayoutRepository,
             IRepository<MatchAppendStatus, int> matchAppendStatusRepository,
             IDatabaseRepository databaseRepository,
             IRepository<BatchQueue, int> batchQueueRepository,
             IShortSearch shortSearch,
             IRepository<Build, int> buildRepository,
             IBuildTableRepository buildTableRepository,
             IRedisIDMSConfigurationCache idmsConfigurationCache,
             IRepository<Lookup, int> lookupRepository,
             IRepository<MatchAppendOutputLayout, int> matchAppendOutputLayoutRepository,
             IBuildTableLayoutRepository customBuildTableLayoutRepository)
        {

            _mySession = mySession;
            _customMatchAppendRepository = customMatchAppendRepository;
            _shortSearch = shortSearch;
            _matchAppendInputLayoutRepository = matchAppendInputLayoutRepository;
            _matchAppendStatusRepository = matchAppendStatusRepository;
            _databaseRepository = databaseRepository;
            _batchQueueRepository = batchQueueRepository;
            _buildRepository = buildRepository;
            _buildTableRepository = buildTableRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
            _lookupRepository = lookupRepository;
            _matchAppendOutputLayoutRepository = matchAppendOutputLayoutRepository;
            _customBuildTableLayoutRepository = customBuildTableLayoutRepository;
        }

        #region fetch tasks
        public PagedResultDto<GetMatchAppendForViewDto> GetAllMatchAppendTasks(GetAllMatchAppendsInput input)
        {
            try
            {
                var shortWhere = _shortSearch.GetWhere(PageID.MatchAppend, input.Filter);
                var query = GetAllMatchAppendTasksQuery(input, _mySession.IDMSUserId, _mySession.IDMSUserName, shortWhere);
                return _customMatchAppendRepository.GetAllMatchAppendTasksList(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Copy Task
        public void CopyMatchAppendTask(int matchAppendId)
        {
            try
            {
                _customMatchAppendRepository.CopyMatchAppendTask(matchAppendId, _mySession.IDMSUserName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        #endregion

         #region Submit Unlock Task
        public void SubmitUnlockMatchAppendTask(int matchAppendId, bool isSubmit)
        {
            try
            {
                var matchAppend = _customMatchAppendRepository.Get(matchAppendId);
                var message = ValidateMatchAppendTask(matchAppend);
                if (!string.IsNullOrEmpty(message))
                    throw new UserFriendlyException(message);
                var matchAppendStatusEntity = new MatchAppendStatus
                {
                    MatchAppendID = matchAppendId,
                    iIsCurrent = true,
                    iStatusID = !isSubmit ? 10 : 20,
                    cCreatedBy = _mySession.IDMSUserName,
                    dCreatedDate = DateTime.Now
                };
                _matchAppendStatusRepository.Insert(matchAppendStatusEntity);
                CurrentUnitOfWork.SaveChanges();

                if (isSubmit)
                {

                    var batchQueueEntity = new BatchQueue
                    {
                        DivisionId = _databaseRepository.FirstOrDefault(a => a.Id == matchAppend.DatabaseID)?.DivisionId ?? 0,
                        ProcessTypeId = BatchProcessType.MatchAppendTask.GetHashCode(),
                        BuildId = matchAppend.BuildID,
                        FieldName = matchAppend.cClientName,
                        ParmData = matchAppend.Id.ToString(),
                        ListId = matchAppend.DatabaseID, //storing databaseID in ListID field.
                        Recipients = _mySession.IDMSUserEmail,
                        dRecordDate = DateTime.Now,
                        dScheduled = DateTime.Now,
                        cScheduledBy = _mySession.IDMSUserName,
                        iPriority = 3,
                        IsStopRequest = false,
                        iStatusId = BatchQueueStatus.Waiting.GetHashCode()
                    };
                    _batchQueueRepository.Insert(batchQueueEntity);
                    CurrentUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetMatchAppendDatabasesBasedOnUserId(int userId)
        {
            try
            {
                var query = GetAllDatabasesFromUseriId(userId);
                return _customMatchAppendRepository.GetAllDatabasesFromUserId(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetLatestBuildFromDatabaseId(int databaseId)
        {
            try
            {
                var buildList = _buildRepository.GetAll().Where(x => x.iIsOnDisk && x.DatabaseId == databaseId).OrderByDescending(x => x.Id);
                var buildDropdown = new List<DropdownOutputDto>();


                foreach (var item in buildList)
                {
                    buildDropdown.Add(new DropdownOutputDto
                    {
                        Label = $"{item.cDescription} ({item.cBuild})",
                        Value = item.Id
                    });
                }
                return buildDropdown;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }



        #endregion

        #region Fetch Tasks

        public List<MatchAndAppendStatusDto> GetMatchAppendStatusHistory(int matchAppendId)
        {
            try
            {
                return _customMatchAppendRepository.GetMatchAppendStatusDetails(matchAppendId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion
        private string ValidateMatchAppendTask(MatchAppend matchAppend)
        {
            var sb = new StringBuilder();
            var errorMsg = string.Empty;
            if (string.IsNullOrEmpty(matchAppend.cClientName))
                sb.Append("Client Name,");
            if (matchAppend.DatabaseID <= 0)
                sb.Append("Database Selection,");
            if (matchAppend.BuildID <= 0)
                sb.Append("Active Build,");
            if (string.IsNullOrEmpty(matchAppend.UploadFilePath))
                sb.Append("Input File,");
            if (string.IsNullOrEmpty(matchAppend.LK_FileType))
                sb.Append("Input File Type,");
            if (string.IsNullOrEmpty(matchAppend.LK_ExportFileFormatID))
                sb.Append("Output Format,");
            if (string.IsNullOrEmpty(matchAppend.cOrderType))
                sb.Append("Order Type,");
            if (string.IsNullOrEmpty(matchAppend.cRequestReason))
                sb.Append("Task Description,");
            if (string.IsNullOrEmpty(matchAppend.cIDMSMatchFieldName))
                sb.Append("IDMS Match Field,");
            if (string.IsNullOrEmpty(matchAppend.cInputMatchFieldName))
                sb.Append("Input Match Field,");
            if (!string.IsNullOrEmpty(errorMsg = sb.ToString()))
                return errorMsg.TrimEnd(',');


            var matchAppendInputlayoutListCount = _matchAppendInputLayoutRepository.GetAll().Count(x => x.MatchAppendId == matchAppend.Id && string.IsNullOrEmpty(x.cFieldName));

            if (matchAppendInputlayoutListCount > 0)
            {
                errorMsg = L("InputLayoutEntryValidation");
            }
            return errorMsg;

        }

        public string GetReadyToLoadPathFromConfiguration(int databaseId)
        {
            try
            {
                return _idmsConfigurationCache.GetConfigurationValue("ReadyToLoadFilesPath", databaseId).cValue;

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public MatchAppendDto ProcessDatabaseSetupStep(MatchAppendDto input, string fileNameFromControl, string fileNameFromFreeText)
        {
            try
            {
                var awsFlag = _idmsConfigurationCache.IsAWSConfigured(input.DatabaseID);
                if (input.Id != 0)
                {
                    if (!string.IsNullOrEmpty(fileNameFromControl))
                    {
                        var ext = Path.GetExtension(fileNameFromControl.Trim()).ToLower();
                        if (awsFlag && ext == ".xlsx")
                            throw new UserFriendlyException(L("ExcelFeatureNotSupported"));

                        if (ext != ".txt" && ext != ".csv" && ext != ".xlsx")
                        {
                            throw new UserFriendlyException(L("IncorrectFileFormatValidation"));
                        }
                    }
                    if (!string.IsNullOrEmpty(fileNameFromControl) && !string.IsNullOrEmpty(fileNameFromFreeText))
                    {
                        throw new UserFriendlyException(L("BothFileUploadsFilledValidation"));
                    }
                    if (!string.IsNullOrEmpty(fileNameFromFreeText))
                    {                      

                        var ext = Path.GetExtension(fileNameFromFreeText.Trim()).ToLower();
                        if (!string.IsNullOrEmpty(ext))
                        {
                            if (awsFlag && ext == ".xlsx")
                                throw new UserFriendlyException(L("ExcelFeatureNotSupported"));

                            if (ext != ".txt" && ext != ".csv" && ext != ".xlsx")
                            {
                                throw new UserFriendlyException(L("IncorrectFileFormatValidation"));
                            }
                            input.cClientFileName = Path.GetFileName(fileNameFromFreeText.Trim());
                            input.UploadFilePath = fileNameFromFreeText.Trim();
                        }
                       
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(fileNameFromControl))
                    {
                        var ext = Path.GetExtension(fileNameFromControl.Trim()).ToLower();
                        if (awsFlag && ext == ".xlsx")
                            throw new UserFriendlyException(L("ExcelFeatureNotSupported"));

                        if (ext != ".txt" && ext != ".csv" && ext != ".xlsx")
                        {
                            throw new UserFriendlyException(L("IncorrectFileFormatValidation"));
                        }
                    }
                    if (!string.IsNullOrEmpty(fileNameFromControl) && !string.IsNullOrEmpty(fileNameFromFreeText))
                    {
                        throw new UserFriendlyException(L("BothFileUploadsFilledValidation"));
                    }

                    if (!string.IsNullOrEmpty(fileNameFromFreeText))
                    {
                        var ext = Path.GetExtension(fileNameFromFreeText.Trim()).ToLower();
                        if (awsFlag && ext == ".xlsx")
                            throw new UserFriendlyException(L("ExcelFeatureNotSupported"));

                        if (ext != ".txt" && ext != ".csv" && ext != ".xlsx")
                        {
                            throw new UserFriendlyException(L("IncorrectFileFormatValidation"));
                        }
                        input.cClientFileName = Path.GetFileName(fileNameFromFreeText.Trim());
                        input.UploadFilePath = fileNameFromFreeText.Trim();
                    }
                    else if (string.IsNullOrEmpty(input.UploadFilePath))
                    {
                        throw new UserFriendlyException(L("ProvideInputFileValidation"));
                    }

                }


                if (input.BuildID == 0)
                {
                    throw new UserFriendlyException(L("DatabaseWithActiveBuildValidation"));
                }
                return input;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void ProcessInputLayoutStep(List<MatchAndAppendInputLayoutDto> matchAppendinputLayout, string fileType, int databaseID)
        {
            try
            {
                var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseID);
                if (string.IsNullOrEmpty(fileType))
                {
                    throw new UserFriendlyException(L("FileTypeNotSelectedValidation"));
                }

                if (awsFlag && fileType == "E")
                    throw new UserFriendlyException(L("ExcelFeatureNotSupported"));

                var checkForEmpty = matchAppendinputLayout.Count(x => !string.IsNullOrEmpty(x.cFieldName) && x.actionType != ActionType.Delete);
                if (checkForEmpty <= 0) throw new UserFriendlyException(L("EnterAtleastOneFieldNameValidation"));
                foreach (var item in matchAppendinputLayout)
                {
                    if (((!string.IsNullOrEmpty(item.cFieldName)) && (!string.IsNullOrEmpty(item.StartIndex)) && (!string.IsNullOrEmpty(item.EndIndex)) && (!string.IsNullOrEmpty(item.DataLength))) || ((!string.IsNullOrEmpty(item.cFieldName)) && (!string.IsNullOrEmpty(item.ImportLayoutOrder))))
                    {
                        if (fileType == "A")
                        {
                            if (string.IsNullOrEmpty(item.StartIndex) && string.IsNullOrEmpty(item.EndIndex))
                            {
                                throw new UserFriendlyException(L("StartIndexAndEndIndexNeeded"));
                            }
                            if (string.IsNullOrEmpty(item.DataLength))
                            {
                                throw new UserFriendlyException(L("WidthNeededValidation"));
                            }
                        }
                    }
                    if (item.cFieldName == "ID")
                    {
                        throw new UserFriendlyException(L("FieldNameShouldNotBeIdValidation"));
                    }
                    var listCfields = matchAppendinputLayout.Where(s => !string.IsNullOrEmpty(s.cFieldName) && s.actionType != ActionType.Delete);
                    var count = listCfields.Count(x => x.cFieldName == item.cFieldName);
                    if (count > 1) throw new UserFriendlyException(L("UniqueFieldValidation"));
                    if (fileType != "A")
                    {
                        var list = matchAppendinputLayout.Where(s => !string.IsNullOrEmpty(s.ImportLayoutOrder) && s.actionType != ActionType.Delete).ToList();
                        var countOrder = list.Count(x => x.ImportLayoutOrder.ToString() == item.ImportLayoutOrder.ToString());
                        if (countOrder > 1) throw new UserFriendlyException(L("UniqueOrderValidation"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }


        public List<DropdownOutputDto> GetMatchAppendFileTypes()
        {
            try
            {
                var result = _lookupRepository.Query(p => p.Where(q => q.cLookupValue.ToUpper().Equals("FILETYPE") && q.iIsActive)).ToList();
                var fileTypeDropdown = new List<DropdownOutputDto>();
                foreach (var item in result)
                {
                    fileTypeDropdown.Add(new DropdownOutputDto
                    {
                        Label = item.cDescription,
                        Value = item.cCode
                    });
                }
                return fileTypeDropdown;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<MatchAndAppendInputLayoutDto> ImportFielList(string importFieldString, int matchAppendId, CreateOrEditMatchAppendDto matchAppendDto)
        {
            try
            {
                var lcFields = string.Empty;
                var lsFields = new List<string>();
                var matchAppendInputLayoutList = new List<MatchAndAppendInputLayoutDto>();
                var lnCurrent = 0;
                if (importFieldString.Trim() != string.Empty)
                {
                    lcFields = importFieldString.Replace("\t", "");
                    lcFields = lcFields.Replace(" ", "");
                    lsFields = lcFields.Trim().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (importFieldString.Trim().Contains(','))
                    {
                        for (lnCurrent = 0; lnCurrent <= lsFields.Count - 1; lnCurrent++)
                        {
                            var sa = lsFields[lnCurrent].Split(',');
                            var lPass = true;


                            lPass = Int32.TryParse(sa[1], out int iOut);
                            if (!lPass)
                            {
                                throw new UserFriendlyException(L("ImportFieldStringNumericValidation", sa[0]));
                            }
                        }
                    }
                    for (lnCurrent = 0; lnCurrent <= lsFields.Count - 1; lnCurrent++)
                    {

                        if (lsFields[lnCurrent].Trim().Length > 50)
                        {
                            throw new UserFriendlyException(L("FieldNameCanNotGreaterThan50Validation"));
                        }
                    }

                }

                var lnMaxOrder = 0;
                var lnInitialCount = 0;

                if (matchAppendId == 0)
                {
                    matchAppendInputLayoutList = new List<MatchAndAppendInputLayoutDto>();
                }
                else
                {
                    matchAppendInputLayoutList = new List<MatchAndAppendInputLayoutDto>();
                    matchAppendInputLayoutList = matchAppendDto.MatchAppendInputLayoutList.Where(x => !string.IsNullOrEmpty(x.cFieldName) && x.actionType != ActionType.Delete).ToList();
                }

                var matchAppendInputLayout = matchAppendInputLayoutList;


                foreach (var obj in matchAppendInputLayoutList)
                {
                    if (obj.cFieldName != string.Empty && obj.actionType != ActionType.Delete)
                    {
                        ++lnInitialCount;
                    }
                }
                var lnCounter = 0;
                if (matchAppendInputLayout.ToList().Count > 0)
                {
                    if (lnInitialCount > 0)
                    {
                        lnMaxOrder = matchAppendInputLayout[lnInitialCount - 1].iImportLayoutOrder;
                    }
                }
                var counter = lsFields.Count + 15;
                for (int i = 0; i < counter; i++)
                {
                    var obj = new MatchAndAppendInputLayoutDto
                    {
                        MatchAppendId = -1,
                        cFieldName = string.Empty,
                        iStartIndex = 0,
                        iEndIndex = 0,
                        iDataLength = 0,
                        iImportLayoutOrder = 0,
                        cMCMapping = string.Empty,
                        actionType = ActionType.None
                    };
                    matchAppendInputLayout.Add(obj);


                }

                string[] lsField;
                for (int miCounter = lnInitialCount; miCounter < (lsFields.Count + lnInitialCount); miCounter++)
                {
                    var lnCurrent1 = lnCounter++;
                    if (lsFields[lnCurrent1] != string.Empty)
                    {
                        if (lsFields[lnCurrent1].Contains(','))
                        {
                            lsField = lsFields[lnCurrent1].Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            matchAppendInputLayout[miCounter].cFieldName = lsField[0].Trim();
                            matchAppendInputLayout[miCounter].iDataLength = Convert.ToInt32(lsField[1]);
                        }
                        else
                        {
                            matchAppendInputLayout[miCounter].cFieldName = lsFields[lnCurrent1];
                        }
                        if (lnMaxOrder == 0)
                        {
                            matchAppendInputLayout[miCounter].iStartIndex = 1;
                        }
                        else if (lnMaxOrder > 0)
                        {
                            matchAppendInputLayout.ToList()[miCounter].iStartIndex = matchAppendInputLayout[miCounter - 1].iEndIndex + 1;
                        }
                        matchAppendInputLayout.ToList()[miCounter].iEndIndex = (matchAppendInputLayout[miCounter].iDataLength + matchAppendInputLayout.ToList()[miCounter].iStartIndex) - 1;
                        matchAppendInputLayout.ToList()[miCounter].iImportLayoutOrder = ++lnMaxOrder;
                        matchAppendInputLayout[miCounter].actionType = ActionType.None;

                    }
                }
                matchAppendInputLayoutList = new List<MatchAndAppendInputLayoutDto>();
                foreach (var item in matchAppendInputLayout.ToList())
                {
                    if (item.actionType == ActionType.Delete)
                    {
                        item.iImportLayoutOrder = lnMaxOrder;
                        item.ImportLayoutOrder = lnMaxOrder.ToString();
                    }
                }
                foreach (var item in matchAppendInputLayout.ToList())
                {
                    var matchAppendLayoutDto = new MatchAndAppendInputLayoutDto
                    {
                        Id = item.Id,
                        MatchAppendId = item.MatchAppendId,
                        cFieldName = item.cFieldName,
                        StartIndex = item.iStartIndex == 0 ? string.Empty : item.iStartIndex.ToString(),
                        EndIndex = item.iEndIndex == 0 ? string.Empty : item.iEndIndex.ToString(),
                        DataLength = item.iDataLength == 0 ? string.Empty : item.iDataLength.ToString(),
                        ImportLayoutOrder = item.iImportLayoutOrder == 0 ? string.Empty : item.iImportLayoutOrder.ToString(),
                        cMCMapping = item.cMCMapping,
                        mappingDescription = GetMappingDescription(item.cMCMapping),
                        actionType = item.actionType
                    };
                    matchAppendInputLayoutList.Add(matchAppendLayoutDto);
                }
                return matchAppendInputLayoutList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetMatchAppendOutputTypes()
        {
            try
            {
                var lookupOutputTypes = _lookupRepository.Query(p => p.Where(q => q.cLookupValue.ToUpper().Equals("EXPORTFILEFORMAT") && q.iIsActive)).ToList();
                var outputTypeDropdown = lookupOutputTypes.Where(x => !x.cDescription.ToLower().Contains("excel")).Select(x => new DropdownOutputDto { Label = x.cDescription, Value = x.cCode }).ToList();
                return outputTypeDropdown;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetBuildTableLayoutFieldByBuildID(string iBuildID)
        {
            try
            {
                return _customBuildTableLayoutRepository.GetBuildTableLayoutFieldByBuildID(iBuildID);

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetSourceDropdownList(int buildId, int databaseId)
        {
            try
            {
                var lstTables = new List<DropdownOutputDto>();
                lstTables.Add(new DropdownOutputDto
                {
                    Label = "Input Table",
                    Value = "-1:Input"
                });

                var tables = _buildTableRepository.GetAll().Where(q => q.BuildId.Equals(buildId)).OrderBy(a => a.ctabledescription).Select(x => new DropdownOutputDto { Label = x.ctabledescription + " (" + x.cTableName.Substring(0, x.cTableName.IndexOf('_')) + ")", Value = x.Id.ToString() + ":" + x.cTableName }).ToList();
                if (tables != null && tables.Count > 0)
                {
                    lstTables.AddRange(tables);
                }
                var lstExternalBuildTables = _buildTableRepository.GetExternalTablesByDatabase(databaseId).OrderBy(a => a.ctabledescription).Select(x => new DropdownOutputDto { Label = x.ctabledescription + " (" + x.cTableName.Substring(0, x.cTableName.IndexOf('_')) + ")", Value = x.Id.ToString() + ":" + x.cTableName }).ToList();
                if (lstExternalBuildTables != null && lstExternalBuildTables.Count > 0)
                {
                    lstTables.AddRange(lstExternalBuildTables);
                }
                var index = lstTables.FindIndex(x => x.Label.Contains("tblMain"));
                if (index >= 0)
                {
                    var item = lstTables[index];
                    lstTables[index] = lstTables[1];
                    lstTables[1] = item;
                }
                return lstTables;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public List<DropdownOutputDto> GetMatchAppendAvailabelFields(string tableId, List<string> list)
        {
            var tableID = tableId.Split(':');
            try
            {
                var availabaleFields = new List<DropdownOutputDto>();

                if (Convert.ToInt32(tableID[0]) != -1)
                {
                    availabaleFields = _customMatchAppendRepository.GetMatchAppendAvailableFields(Convert.ToInt32(tableID[0]));
                }
                else
                {
                    availabaleFields.Add(new DropdownOutputDto
                    {
                        Label = "Key Code",
                        Value = "KEYCODE"
                    });
                    foreach (var item in list)
                    {
                        availabaleFields.Add(new DropdownOutputDto
                        {
                            Value = item,
                            Label = item
                        });
                    }

                }
                return availabaleFields;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public List<MatchAndAppendOutputLayoutDto> AddMatchAppendSelectedFields(CreateOrEditMatchAppendDto matchAppendDto, int matchAppendId)
        {
            try
            {
                var outputLayoutList = new List<MatchAndAppendOutputLayoutDto>();
                var dataLen = "15";
                var fieldDesc = string.Empty;
                string[] selValue = matchAppendDto.SelectedTable.Split(':');
                int tableID = int.Parse(selValue[0]);
                int iOutputLayoutOrder = matchAppendDto.MatchAppendOutputLayoutList.Count(x => x.ActionType != ActionType.Delete);
                foreach (var item in matchAppendDto.SelectedFields)
                {
                    if (tableID != -1)
                    {
                        var buildLayout = _customBuildTableLayoutRepository.GetMatchAppendFieldDetails(item, tableID);
                        dataLen = buildLayout.cWidth;
                        fieldDesc = buildLayout.cFieldDescription;


                    }
                    else
                    {
                        if (matchAppendDto.MatchAppendDto.LK_FileType == "A")
                        {
                            if (item == "KEYCODE")
                            {
                                fieldDesc = "Key Code";
                                dataLen = "50";
                            }
                            else
                            {
                                var inputLayoutObject = matchAppendDto.MatchAppendInputLayoutList.FirstOrDefault(x => x.cFieldName == item);
                                dataLen = inputLayoutObject.DataLength;
                            }

                        }
                        else
                        {
                            dataLen = "50";
                        }
                        if (item == "KEYCODE")
                        {
                            fieldDesc = "Key Code";
                            dataLen = "50";
                        }
                        else
                        {
                            fieldDesc = item;
                        }

                    }
                    if (dataLen == "0")
                        dataLen = "15";

                    iOutputLayoutOrder = iOutputLayoutOrder + 1;

                    outputLayoutList.Add(new MatchAndAppendOutputLayoutDto
                    {
                        MatchAppendID = matchAppendId,
                        cTableName = selValue[1],
                        cFieldName = item,
                        cOutputFieldName = fieldDesc,
                        OutputLength = dataLen,
                        OutputLayoutOrder = iOutputLayoutOrder.ToString()


                    });
                }


                return outputLayoutList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }





        public async Task<GetMatchAppendForEditOutput> GetMatchAppendForEdit(int matchAppendId)
        {
            var matchAppend = await _customMatchAppendRepository.FirstOrDefaultAsync(matchAppendId);
            var matchAppendDto = ObjectMapper.Map<MatchAppendDto>(matchAppend);

            var matchAppendinputLayoutList = _matchAppendInputLayoutRepository.GetAll().Where(x => x.MatchAppendId == matchAppendId);


            var inputLayoutlist = new List<MatchAndAppendInputLayoutDto>();
            foreach (var item in matchAppendinputLayoutList)
            {
                var matchAppendInputLayoutDto = ObjectMapper.Map<MatchAndAppendInputLayoutDto>(item);
                matchAppendInputLayoutDto.cCreatedBy = matchAppendDto.cCreatedBy;
                matchAppendInputLayoutDto.dCreatedDate = matchAppendDto.dCreatedDate;
                matchAppendInputLayoutDto.dModifiedDate = matchAppendDto.dModifiedDate;
                matchAppendInputLayoutDto.StartIndex = item.iStartIndex == 0 ? string.Empty : item.iStartIndex.ToString();
                matchAppendInputLayoutDto.EndIndex = item.iEndIndex == 0 ? string.Empty : item.iEndIndex.ToString();
                matchAppendInputLayoutDto.DataLength = item.iDataLength == 0 ? string.Empty : item.iDataLength.ToString();
                matchAppendInputLayoutDto.mappingDescription = GetMappingDescription(item.cMCMapping);
                matchAppendInputLayoutDto.ImportLayoutOrder = item.iImportLayoutOrder == 0 ? string.Empty : item.iImportLayoutOrder.ToString();
                inputLayoutlist.Add(matchAppendInputLayoutDto);

            }

            var matchAppendOutputLayoutList = _matchAppendOutputLayoutRepository.GetAll().Where(x => x.MatchAppendID == matchAppendId);
            var outputLayoutList = new List<MatchAndAppendOutputLayoutDto>();
            foreach (var item in matchAppendOutputLayoutList)
            {
                var matchAppendoutputLayoutdto = ObjectMapper.Map<MatchAndAppendOutputLayoutDto>(item);
                matchAppendoutputLayoutdto.dCreatedDate = matchAppendDto.dCreatedDate;
                matchAppendoutputLayoutdto.dModifiedDate = matchAppendDto.dModifiedDate;
                matchAppendoutputLayoutdto.cOutputFieldName = item.cOutputFieldName;
                matchAppendoutputLayoutdto.cCreatedBy = item.cCreatedBy;
                matchAppendoutputLayoutdto.dCreatedDate = item.dCreatedDate;
                matchAppendoutputLayoutdto.OutputLayoutOrder = item.iOutputLayoutOrder == 0 ? "" : item.iOutputLayoutOrder.ToString();
                matchAppendoutputLayoutdto.OutputLength = item.iOutputLength == 0 ? "" : item.iOutputLength.ToString();
                outputLayoutList.Add(matchAppendoutputLayoutdto);
            }

            var output = new GetMatchAppendForEditOutput();
            output.MatchAppend = new CreateOrEditMatchAppendDto();
            output.MatchAppend.MatchAppendDto = new MatchAppendDto();
            output.MatchAppend.MatchAppendInputLayoutList = new List<MatchAndAppendInputLayoutDto>();
            output.MatchAppend.MatchAppendOutputLayoutList = new List<MatchAndAppendOutputLayoutDto>();
            output.MatchAppend.MatchAppendDto = matchAppendDto;
            output.MatchAppend.MatchAppendInputLayoutList = inputLayoutlist;
            output.MatchAppend.MatchAppendOutputLayoutList = outputLayoutList;

            return output;
        }

        private string GetMappingDescription(string mappingCode)
        {
            switch (mappingCode)
            {
                case "L": return "Last Name";
                case "F": return "First Name";
                case "A": return "Address Line 1";
                case "C": return "Company";
                case "Z": return "Zip";
                default: return string.Empty;
            }
        }

        public void CreateOrEdit(CreateOrEditMatchAppendDto input)
        {
            try
            {
                ValidateOnFinish(input);
                if (input.MatchAppendDto.Id == 0)
                {
                    Create(input);
                }
                else
                {
                    Update(input);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        public void CreateEditAndSubmit(CreateOrEditMatchAppendDto input)
        {
            try
            {
                ValidateOnFinish(input);
                if (input.MatchAppendDto.Id == 0)
                {
                    Create(input);
                   
                }
                else
                {
                    Update(input);
                }
                SubmitUnlockMatchAppendTask(input.MatchAppendDto.Id, true);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private void ValidateOnFinish(CreateOrEditMatchAppendDto input)
        {
            var inputKey = input.MatchAppendDto.cInputMatchFieldName;
            if (inputKey == "Individual_MC" || inputKey == "Household_MC" || inputKey == "Company_MC" || inputKey=="Address_MC")
            {
                switch (inputKey)
                {
                    case "Individual_MC":
                        var licount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "L" && x.actionType != ActionType.Delete);
                        var ficount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "F" && x.actionType != ActionType.Delete);
                        var aicount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "A" && x.actionType != ActionType.Delete);
                        var zicount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "Z" && x.actionType != ActionType.Delete);
                        var itotalcount = licount + ficount + aicount + zicount;
                        if (itotalcount != 4) throw new UserFriendlyException(L("InvividualMCValidation"));
                        break;

                    case "Household_MC":
                        var lhcount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "L" && x.actionType != ActionType.Delete);
                        var ahcount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "A" && x.actionType != ActionType.Delete);
                        var zhcount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "Z" && x.actionType != ActionType.Delete);
                        var htotalcount = lhcount + ahcount + zhcount;
                        if (htotalcount != 3) throw new UserFriendlyException(L("HouseholdMCValidation"));
                        break;
                    case "Company_MC":
                        var cccount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "C" && x.actionType != ActionType.Delete);
                        var account = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "A" && x.actionType != ActionType.Delete);
                        var zccount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "Z" && x.actionType != ActionType.Delete);
                        var ctotalcount = cccount + account + zccount;
                        if (ctotalcount != 3) throw new UserFriendlyException(L("CompanyValidation"));
                        break;

                    case "Address_MC":
                       
                        var aacount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "A" && x.actionType != ActionType.Delete);
                        var zacount = input.MatchAppendInputLayoutList.Count(x => x.cMCMapping == "Z" && x.actionType != ActionType.Delete);
                        var atotalcount = aacount + zacount;
                        if (atotalcount != 2) throw new UserFriendlyException(L("AddressMCValidation"));
                        break;
                    default: break;


                }
                if (input.MatchAppendOutputLayoutList == null || input.MatchAppendOutputLayoutList.Count == 0)
                {
                    throw new UserFriendlyException(L("OutputLayoutBlankValidation"));
                }
            }
        }

        [AbpAuthorize(AppPermissions.Pages_MatchAppends_Create)]
        protected void Create(CreateOrEditMatchAppendDto input)
        {
            try
            {
                input.MatchAppendDto = CommonHelpers.ConvertNullStringToEmptyAndTrim(input.MatchAppendDto);

                input.MatchAppendDto.cCreatedBy = _mySession.IDMSUserName;
                input.MatchAppendDto.dCreatedDate = DateTime.Now;


                var matchAppend = ObjectMapper.Map<MatchAppend>(input.MatchAppendDto);
                var matchAppendId = _customMatchAppendRepository.InsertAndGetId(matchAppend);
                CurrentUnitOfWork.SaveChanges();

                if (input.MatchAppendDto.UploadFilePath.StartsWith("@#MOVE#@"))
                {
                    var path = string.Empty;
                    string filename = input.MatchAppendDto.UploadFilePath.Replace("@#MOVE#@", "");
                    var awsFlag = _idmsConfigurationCache.IsAWSConfigured(input.MatchAppendDto.DatabaseID);
                    if (awsFlag)
                    {                        
                        var rx = new Regex(_s3PathRegex).Match(filename);
                        var bucket = rx.Groups["bucket"].Value;
                        var srckey = rx.Groups["key"].Value;                        
                        var destfname = $"MatchAppend_{input.MatchAppendDto.DatabaseID.ToString()}_{input.MatchAppendDto.BuildID.ToString()}_{matchAppendId.ToString()}{Path.GetExtension(filename)}";
                        var destkey = srckey.Replace(Path.GetFileName(srckey), destfname);
                        var s3Util = new S3Utilities();
                        s3Util.MoveObject(bucket, srckey, bucket, destkey);
                        var fname = Path.GetFileName(filename);
                        path = filename.Replace(fname, destfname);
                    }
                    else
                    {
                        path = Path.GetDirectoryName(filename);
                        path = $@"{path}\MatchAppend_{input.MatchAppendDto.DatabaseID.ToString()}_{input.MatchAppendDto.BuildID.ToString()}_{matchAppendId.ToString()}{Path.GetExtension(filename)}";
                        if (File.Exists(path.Replace("|", "")))
                            File.Delete(path.Replace("|", ""));
                        File.Move(filename.Replace("|", ""), path.Replace("|", ""));
                    }
                    var matchAppendUpdate = _customMatchAppendRepository.Get(matchAppendId);
                    matchAppendUpdate.UploadFilePath = path;
                    _customMatchAppendRepository.Update(matchAppendUpdate);
                    CurrentUnitOfWork.SaveChanges();
                }

                var matchAppendStatus = new MatchAppendStatus
                {
                    MatchAppendID = matchAppendId,
                    iStatusID = 10,
                    iIsCurrent = true,
                    cCreatedBy = _mySession.IDMSUserName,
                    dCreatedDate = DateTime.Now
                };
                _matchAppendStatusRepository.Insert(matchAppendStatus);
                CurrentUnitOfWork.SaveChanges();

                input.MatchAppendInputLayoutList = input.MatchAppendInputLayoutList.Where(x => !string.IsNullOrEmpty(x.cFieldName) && x.actionType != ActionType.Delete).ToList();
                foreach (var inputLayoutItem in input.MatchAppendInputLayoutList)
                {

                    var inputLayout = CommonHelpers.ConvertNullStringToEmptyAndTrim(inputLayoutItem);
                    inputLayout.iStartIndex = string.IsNullOrEmpty(inputLayout.StartIndex) ? 0 : Convert.ToInt32(inputLayout.StartIndex);
                    inputLayout.iEndIndex = string.IsNullOrEmpty(inputLayout.EndIndex) ? 0 : Convert.ToInt32(inputLayout.EndIndex);
                    inputLayout.iDataLength = string.IsNullOrEmpty(inputLayout.DataLength) ? 0 : Convert.ToInt32(inputLayout.DataLength);
                    if(matchAppend.LK_FileType!="A")
                    {
                        inputLayout.iDataLength = 150;
                    }
                    inputLayout.iImportLayoutOrder = string.IsNullOrEmpty(inputLayout.ImportLayoutOrder) ? 0 : Convert.ToInt32(inputLayout.ImportLayoutOrder);
                    inputLayout.cCreatedBy = _mySession.IDMSUserName;
                    inputLayout.dCreatedDate = DateTime.Now;
                    inputLayout.MatchAppendId = matchAppendId;



                    var matchAppendInputLayout = ObjectMapper.Map<MatchAppendInputLayout>(inputLayout);
                    _matchAppendInputLayoutRepository.Insert(matchAppendInputLayout);
                    CurrentUnitOfWork.SaveChanges();

                }

                input.MatchAppendOutputLayoutList = input.MatchAppendOutputLayoutList.Where(x => x.ActionType != ActionType.Delete).ToList();
                foreach (var outputLayoutItem in input.MatchAppendOutputLayoutList)
                {

                    var outputLayout = CommonHelpers.ConvertNullStringToEmptyAndTrim(outputLayoutItem);

                    outputLayout.iOutputLength = string.IsNullOrEmpty(outputLayout.OutputLength) ? 0 : Convert.ToInt32(outputLayout.OutputLength);
                    outputLayout.iOutputLayoutOrder = string.IsNullOrEmpty(outputLayout.OutputLayoutOrder) ? 0 : Convert.ToInt32(outputLayout.OutputLayoutOrder);
                    outputLayout.cCreatedBy = _mySession.IDMSUserName;
                    outputLayout.dCreatedDate = DateTime.Now;
                    outputLayout.MatchAppendID = matchAppendId;

                    if (outputLayout.iOutputLength > 0)
                    {
                        var matchAppendInputLayout = ObjectMapper.Map<MatchAppendOutputLayout>(outputLayout);
                        _matchAppendOutputLayoutRepository.Insert(matchAppendInputLayout);
                        CurrentUnitOfWork.SaveChanges();
                    }

                }

                input.MatchAppendDto.Id = matchAppendId;
            }

            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_MatchAppends_Edit)]
        protected void Update(CreateOrEditMatchAppendDto input)
        {
            try
            {
                var matchAppend = _customMatchAppendRepository.Get(input.MatchAppendDto.Id);
                input.MatchAppendDto.cModifiedBy = _mySession.IDMSUserName;
                input.MatchAppendDto.dModifiedDate = DateTime.Now;
                
                if (input.MatchAppendDto.UploadFilePath.StartsWith("@#MOVE#@"))
                {
                    var path = string.Empty;
                    string filename = input.MatchAppendDto.UploadFilePath.Replace("@#MOVE#@", "");
                    var awsFlag = _idmsConfigurationCache.IsAWSConfigured(input.MatchAppendDto.DatabaseID);
                    if (awsFlag)
                    {
                        var rx = new Regex(_s3PathRegex).Match(filename);
                        var bucket = rx.Groups["bucket"].Value;
                        var srckey = rx.Groups["key"].Value;
                        var destfname = $"MatchAppend_{input.MatchAppendDto.DatabaseID.ToString()}_{input.MatchAppendDto.BuildID.ToString()}_{input.MatchAppendDto.Id.ToString()}{Path.GetExtension(filename)}";
                        var destkey = srckey.Replace(Path.GetFileName(srckey), destfname);
                        var s3Util = new S3Utilities();
                        s3Util.MoveObject(bucket, srckey, bucket, destkey);
                        var fname = Path.GetFileName(filename);
                        path = filename.Replace(fname, destfname);
                    }
                    else
                    {
                        path = Path.GetDirectoryName(filename);
                        path = $@"{path}\MatchAppend_{input.MatchAppendDto.DatabaseID.ToString()}_{input.MatchAppendDto.BuildID.ToString()}_{input.MatchAppendDto.Id.ToString()}{Path.GetExtension(filename)}";
                        if (File.Exists(path.Replace("|", "")))
                            File.Delete(path.Replace("|", ""));
                        File.Move(filename.Replace("|", ""), path.Replace("|", ""));
                    }
                    input.MatchAppendDto.UploadFilePath = path;
                    
                }
                ObjectMapper.Map(input.MatchAppendDto, matchAppend);
                CurrentUnitOfWork.SaveChanges();

                var deleteRecords = input.MatchAppendInputLayoutList.Where(x => x.actionType == ActionType.Delete);
                foreach (var item in deleteRecords)
                {
                    _matchAppendInputLayoutRepository.Delete(item.Id);
                }
                input.MatchAppendInputLayoutList.RemoveAll(x => x.actionType == ActionType.Delete);


                input.MatchAppendInputLayoutList = input.MatchAppendInputLayoutList.Where(x => !string.IsNullOrEmpty(x.cFieldName)).ToList();
                foreach (var inputLayout in input.MatchAppendInputLayoutList)
                {
                    if (inputLayout.Id != 0)
                    {
                        var matchAppendInputLayout = _matchAppendInputLayoutRepository.Get(inputLayout.Id);
                        inputLayout.cCreatedBy = matchAppendInputLayout.cCreatedBy;
                        inputLayout.dCreatedDate = matchAppendInputLayout.dCreatedDate;
                        inputLayout.iStartIndex = string.IsNullOrEmpty(inputLayout.StartIndex) ? 0 : Convert.ToInt32(inputLayout.StartIndex);
                        inputLayout.iEndIndex = string.IsNullOrEmpty(inputLayout.EndIndex) ? 0 : Convert.ToInt32(inputLayout.EndIndex);
                        inputLayout.iDataLength = string.IsNullOrEmpty(inputLayout.DataLength) ? 0 : Convert.ToInt32(inputLayout.DataLength);
                        if (matchAppend.LK_FileType != "A")
                        {
                            inputLayout.iDataLength = 150;
                        }
                        inputLayout.iImportLayoutOrder = string.IsNullOrEmpty(inputLayout.ImportLayoutOrder) ? 0 : Convert.ToInt32(inputLayout.ImportLayoutOrder);
                        inputLayout.cModifiedBy = _mySession.IDMSUserName;
                        inputLayout.dModifiedDate = DateTime.Now;
                        ObjectMapper.Map(inputLayout, matchAppendInputLayout);
                        CurrentUnitOfWork.SaveChanges();
                    }
                    else
                    {
                        var inputLayout1 = CommonHelpers.ConvertNullStringToEmptyAndTrim(inputLayout);
                        inputLayout1.Id = 0;
                        inputLayout1.iStartIndex = string.IsNullOrEmpty(inputLayout1.StartIndex) ? 0 : Convert.ToInt32(inputLayout1.StartIndex);
                        inputLayout1.iEndIndex = string.IsNullOrEmpty(inputLayout1.EndIndex) ? 0 : Convert.ToInt32(inputLayout1.EndIndex);
                        inputLayout1.iDataLength = string.IsNullOrEmpty(inputLayout1.DataLength) ? 0 : Convert.ToInt32(inputLayout1.DataLength);
                        inputLayout1.iImportLayoutOrder = string.IsNullOrEmpty(inputLayout1.ImportLayoutOrder) ? 0 : Convert.ToInt32(inputLayout1.ImportLayoutOrder);
                        inputLayout1.cCreatedBy = _mySession.IDMSUserName;
                        inputLayout1.dCreatedDate = DateTime.Now;
                        inputLayout1.MatchAppendId = input.MatchAppendDto.Id;



                        var matchAppendInputLayout = ObjectMapper.Map<MatchAppendInputLayout>(inputLayout1);
                        _matchAppendInputLayoutRepository.Insert(matchAppendInputLayout);
                        CurrentUnitOfWork.SaveChanges();
                    }
                }
                var deleteOutputLayoutRecords = input.MatchAppendOutputLayoutList.Where(x => x.ActionType == ActionType.Delete);
                foreach (var item in deleteOutputLayoutRecords)
                {
                    _matchAppendOutputLayoutRepository.Delete(item.Id);
                }
                input.MatchAppendOutputLayoutList.RemoveAll(x => x.ActionType == ActionType.Delete);
                input.MatchAppendOutputLayoutList = input.MatchAppendOutputLayoutList.Where(x => !string.IsNullOrEmpty(x.cOutputFieldName)).ToList();
                foreach (var outputLayout in input.MatchAppendOutputLayoutList)
                {
                    if (outputLayout.Id != 0)
                    {
                        var matchAppendOutputLayout = _matchAppendOutputLayoutRepository.Get(outputLayout.Id);
                        outputLayout.iOutputLength = string.IsNullOrEmpty(outputLayout.OutputLength) ? 0 : Convert.ToInt32(outputLayout.OutputLength);
                        outputLayout.iOutputLayoutOrder = string.IsNullOrEmpty(outputLayout.OutputLayoutOrder) ? 0 : Convert.ToInt32(outputLayout.OutputLayoutOrder);
                        outputLayout.cModifiedBy = _mySession.IDMSUserName;
                        outputLayout.dModifiedDate = DateTime.Now;
                        if (outputLayout.iOutputLength > 0)
                        {
                            ObjectMapper.Map(outputLayout, matchAppendOutputLayout);
                            CurrentUnitOfWork.SaveChanges();
                        }
                    }
                    else
                    {
                        var outputLayout1 = CommonHelpers.ConvertNullStringToEmptyAndTrim(outputLayout);

                        outputLayout1.iOutputLength = string.IsNullOrEmpty(outputLayout1.OutputLength) ? 0 : Convert.ToInt32(outputLayout1.OutputLength);
                        outputLayout1.iOutputLayoutOrder = string.IsNullOrEmpty(outputLayout1.OutputLayoutOrder) ? 0 : Convert.ToInt32(outputLayout1.OutputLayoutOrder);
                        outputLayout1.cCreatedBy = _mySession.IDMSUserName;
                        outputLayout1.dCreatedDate = DateTime.Now;
                        outputLayout1.MatchAppendID = outputLayout1.MatchAppendID;

                        if (outputLayout1.iOutputLength > 0)
                        {
                            var matchAppendInputLayout = ObjectMapper.Map<MatchAppendOutputLayout>(outputLayout1);
                            _matchAppendOutputLayoutRepository.Insert(matchAppendInputLayout);
                            CurrentUnitOfWork.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }



        private static Tuple<string, string, List<SqlParameter>> GetAllMatchAppendTasksQuery(GetAllMatchAppendsInput filters, int currentUserId, string userName, string shortWhere)
        {

            if (!string.IsNullOrEmpty(shortWhere))
                filters.Filter = "";

            if (!string.IsNullOrEmpty(filters.Filter))
                filters.Filter = filters.Filter.Trim();

            string[] filtersarray = null;
            var isModelId = Validation.ValidationHelper.IsNumeric(filters.Filter);

            if (!string.IsNullOrEmpty(filters.Filter))
                filtersarray = filters.Filter.Split(',');

            var defaultFilter = $@"AND (cDatabaseName LIKE @FilterText OR B.cDescription + ' (' + b.cBuild + ')' LIKE @FilterText OR cClientName LIKE @FilterText OR cRequestReason Like @FilterText)";

            var query = new Common.QueryBuilder();
            query.AddSelect($"MA.*, MAS.iStatusID, CASE MAS.iStatusID WHEN 10 THEN 'New' WHEN 20 THEN 'Submitted' WHEN 30 THEN 'Running' WHEN 40 THEN 'Completed' WHEN 50 THEN 'Failed' ELSE 'Undefined' END AS CodeStatus, MAS.dCreatedDate AS[StatusDate],D.cDatabaseName, B.cDescription + ' (' + b.cBuild + ')' as BuildAndDesc");
            query.AddFrom("tblMatchAppend ", "MA");
            query.AddJoin("tblMatchAppendStatus", "MAS", "Id", "MA", "INNER JOIN", "MatchAppendID").And("MAS.iIsCurrent", "EQUALTO", "1");
            query.AddJoin("tblDatabase", "D", "DatabaseID", "MA", "INNER JOIN", "ID");
            query.AddJoin("tblBuild", "B", "BuildID", "MA", "INNER JOIN", "ID");
            query.AddJoin("tblMatchAppendDatabaseUser ", "UD", "ID", "D", "INNER JOIN", "DatabaseID");
            query.AddWhere("", "UD.UserID", "EQUALTO", currentUserId.ToString());
            if (shortWhere.Length > 0)
                query.AddWhereString($"AND ({shortWhere})");
            else
            {
                if (isModelId)
                    query.AddWhere("AND", "MA.ID", "IN", filtersarray);
                else
                    query.AddWhereString(defaultFilter);
            }
            if (!string.IsNullOrEmpty(filters.UserNameFiler))
            {

                query.AddWhere("AND", "MA.cCreatedBy", "EQUALTO", userName);
            }



            query.AddSort(filters.Sorting ?? "'StatusDate' DESC");
            query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
            query.AddDistinct();
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));

            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
        }

        private static Tuple<string, List<SqlParameter>> GetAllDatabasesFromUseriId(int userId)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect($"D.cDatabaseName,D.ID");
            query.AddFrom("tblMatchAppendDatabaseUser", "UD");
            query.AddJoin("tblDatabase", "D", "DatabaseID", "UD", "INNER JOIN", "ID");

            query.AddWhere("", "UserID", "EQUALTO", userId.ToString());
            (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sqlSelect.ToString(), sqlParams);
        }


    }
}