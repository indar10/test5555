using System.Linq;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Infogroup.IDMS.BuildTableLayouts;
using Abp.UI;
using System;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.CampaignExportLayouts;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.ExportLayoutDetails;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.ExportLayouts.Exporting;
using Microsoft.AspNetCore.Mvc;
using Infogroup.IDMS.OrderStatuss;
using System.Data.SqlClient;
using Infogroup.IDMS.IDMSUsers;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using Syncfusion.XlsIO;
using System.Data;
using System.Diagnostics;

namespace Infogroup.IDMS.ExportLayouts
{

    public class ExportLayoutsAppService : IDMSAppServiceBase, IExportLayoutsAppService
    {

        private readonly IRepository<Lookup, int> _lookupRepository;
        private readonly IExportLayoutRepository _exportLayoutRepository;
        private readonly IExportLayoutsRepository _maintainanceExportLayoutRepository;
        private readonly IRepository<ExportLayoutDetail, int> _maintainanceExportLayoutDetailRepository;
        private readonly IDatabaseRepository _databaseRpository;
        private readonly IBuildTableRepository _buildRepository;
        private readonly IRepository<Build, int> _buildsRepository;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly ICampaignRepository _customCampaignRepository;
        private readonly IBuildTableLayoutRepository _buildTableLayoutRepository;
        private readonly AppSession _mySession;
        private readonly ILayoutExcelExporter _layoutExcelExporter;
        private readonly IOrderStatusManager _orderStatusManager;
        private readonly IRedisIDMSUserCache _userCache;
        private readonly IIDMSPermissionChecker _permissionChecker;


        public ExportLayoutsAppService(
            IDatabaseRepository databaseRepository,
            IRepository<Lookup, int> lookupRepository,
            IBuildTableRepository buildRepository,
            ICampaignRepository customCampaignRepository,
            IBuildTableLayoutRepository buildTableLayoutRepository,
            IRedisIDMSConfigurationCache idmsConfigurationCache,
            AppSession mySession,
            IExportLayoutRepository exportLayoutRepository,
            IExportLayoutsRepository maintainanceExportLayoutRepository,
            IRepository<Build, int> buildsRepository,
            IRepository<ExportLayoutDetail, int> maintainanceExportLayoutDetailRepository,
            ILayoutExcelExporter layoutExcelExporter,
            IOrderStatusManager orderStatusManager,
            IRedisIDMSUserCache userCache,
            IIDMSPermissionChecker permissionChecker

            )
        {
            _databaseRpository = databaseRepository;
            _lookupRepository = lookupRepository;
            _buildRepository = buildRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
            _customCampaignRepository = customCampaignRepository;
            _buildTableLayoutRepository = buildTableLayoutRepository;
            _mySession = mySession;
            _exportLayoutRepository = exportLayoutRepository;
            _maintainanceExportLayoutRepository = maintainanceExportLayoutRepository;
            _buildsRepository = buildsRepository;
            _maintainanceExportLayoutDetailRepository = maintainanceExportLayoutDetailRepository;
            _layoutExcelExporter = layoutExcelExporter;
            _orderStatusManager = orderStatusManager;
            _userCache = userCache;
            _permissionChecker = permissionChecker;
        }


        #region Edit export layout
        public List<DropdownOutputDto> GetOutputCaseDropDownValues()
        {
            try
            {
                var lstLookUp = _lookupRepository.Query(p => p.Where(q => q.cLookupValue.Equals("COUTPUTCASE") && q.iIsActive)).OrderBy(a => a.cDescription).Select(x => new DropdownOutputDto { Label = x.cDescription, Value = x.cCode }).ToList();
                return lstLookUp;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetTableDescriptionDropDownValues(int campaignId, int maintenanceBuildId, bool isCampaign, int databaseId)
        {
            try
            {
                var buildId = 0;
                if (!isCampaign)
                {
                    buildId = maintenanceBuildId;
                }
                else
                {
                    buildId = _customCampaignRepository.Get(campaignId).BuildID;
                }
                var lstTables = new List<DropdownOutputDto>();
                var lstExternalBuildTables = new List<DropdownOutputDto>();
                lstTables = _buildRepository.GetAll().Where(q => q.BuildId.Equals(buildId)).Select(x => new DropdownOutputDto { Label = x.ctabledescription + " (" + x.cTableName.Substring(0, x.cTableName.IndexOf('_')) + ")", Value = x.Id }).ToList();
                lstExternalBuildTables = isCampaign ? _buildRepository.GetExternalTables(campaignId).Select(x => new DropdownOutputDto { Label = x.ctabledescription + " (" + x.cTableName.Substring(0, x.cTableName.IndexOf('_')) + ")", Value = x.Id }).ToList() : _buildRepository.GetExternalTablesByDatabase(databaseId).Select(x => new DropdownOutputDto { Label = x.ctabledescription + " (" + x.cTableName.Substring(0, x.cTableName.IndexOf('_')) + ")", Value = x.Id }).ToList();
                if (lstExternalBuildTables != null && lstExternalBuildTables.Count > 0)
                {
                    lstTables.AddRange(lstExternalBuildTables);
                }
                return lstTables;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetExportLayoutAddField(int tableId, int campaignId, bool isCampaign, int exportLayoutId, int maintainanceBuildId, int databaseId)
        {
            try
            {
                var lstFields = new List<DropdownOutputDto>();
                var buildId = 0;
                var databaseID = 0;
                if (isCampaign)
                {
                    lstFields = _buildRepository.GetExportLayoutAddFields(tableId, campaignId).OrderBy(a => a.FieldDescription).Select(xtab => new DropdownOutputDto { Label = xtab.FieldDescription, Value = xtab.FieldName }).ToList();
                    buildId = _customCampaignRepository.Get(campaignId).BuildID;
                    databaseID = _databaseRpository.GetDataSetDatabaseByOrderID(campaignId).Id;
                }
                else
                {
                    lstFields = _maintainanceExportLayoutRepository.GetExportLayoutAddFields(tableId, exportLayoutId).OrderBy(a => a.FieldDescription).Select(xtab => new DropdownOutputDto { Label = xtab.FieldDescription, Value = xtab.FieldName }).ToList();
                    buildId = maintainanceBuildId;
                    databaseID = databaseId;
                }
                lstFields.Add(new DropdownOutputDto { Label = "Key Code 1", Value = "tblSegment.cKeyCode1" });
                lstFields.Add(new DropdownOutputDto { Label = "Key Code 2", Value = "tblSegment.cKeyCode2" });
                lstFields.Add(new DropdownOutputDto { Label = "Distance", Value = "tblSegment.distance" });

                var specialFields = _idmsConfigurationCache.GetConfigurationValue("SpecialField", databaseID).cValue;
                if (!string.IsNullOrEmpty(specialFields))
                {
                    var fields = specialFields.Split('|');
                    foreach (string field in fields)
                    {
                        var dispValue = field.Substring(0, field.IndexOf(':'));
                        if (dispValue == "SpecialSIC")

                            lstFields.Add(new DropdownOutputDto { Label = "SelectedSIC", Value = "tblSegment." + dispValue });
                        else
                            lstFields.Add(new DropdownOutputDto { Label = dispValue, Value = "tblSegment." + dispValue });
                        if (dispValue.ToLower().Contains("sic"))
                            lstFields.Add(new DropdownOutputDto { Label = "SIC Description", Value = "tblSegment.SICDescription" });
                    }
                }
                lstFields.Add(new DropdownOutputDto { Label = "IDMS #", Value = "tblSegment.IDMSNumber" });
                lstFields.Add(new DropdownOutputDto { Label = "Custom...", Value = "" });
                lstFields.RemoveAll(x => x.Label.Equals("Phone"));
                return lstFields;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public Campaign GetCampaignRecordForCampaignId(int campaignId, int databaseID)
        {
            try
            {
                if (!_permissionChecker.IsGranted(_mySession.IDMSUserId, databaseID, PermissionList.CampaignOutputLayout, AccessLevel.iAddEdit))
                    throw new UserFriendlyException(L("ExportLayoutDatabasePermissionError"));
                return _customCampaignRepository.Get(campaignId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void SaveLayoutOutputCase(int camapignId, string outputCase, int campaignStatus)
        {
            try
            {
                var campaign = _customCampaignRepository.Get(camapignId);
                campaign.cOutputCase = outputCase;
                _customCampaignRepository.UpdateAsync(campaign);
                UpdateCampaignStatus(camapignId, campaignStatus);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [HttpGet]
        public List<GetExportLayoutSelectedFieldsDto> AddNewSelectedFields(string selectedFields, int tableId, int campaignId, int maintainanceBuildId, bool isCampaign, int exportLayoutId, int databaseId, int campaignStatus)
        {
            try
            {
                var fields = selectedFields.Split(",");
                var listSelectedFields = new List<GetExportLayoutSelectedFieldsDto>();
                var selectedFieldList = GetExportLayoutSelectedFields(campaignId, isCampaign, exportLayoutId, maintainanceBuildId);
                var maxOrderId = selectedFieldList.Count > 0 ? selectedFieldList.Select(x => x.Order).Max() : 0;
                var tableName = string.Empty;
                var tableDescription = string.Empty;
                var tableList = GetTableDescriptionDropDownValues(campaignId, maintainanceBuildId, isCampaign, databaseId);
                foreach (var item in tableList)
                {
                    if (Convert.ToInt32(item.Value) == tableId)
                    {
                        var tableEntity = _buildRepository.Get(tableId).LK_TableType;
                        tableDescription = item.Label.ToString().Substring(0, item.Label.IndexOf("("));
                        if (tableEntity.Equals("M"))
                        {
                            tableName = "MainTable";
                        }
                        else
                            tableName = item.Label.ToString().Split('(').Last().TrimEnd(')');
                    }
                }
                foreach (var item in fields)
                {
                    var selectedField = new GetExportLayoutSelectedFieldsDto();
                    maxOrderId = maxOrderId + 1;
                    selectedField.Order = maxOrderId;
                    selectedField.Width = _buildTableLayoutRepository.GetFieldName(item, tableId).Width;
                    selectedField.Width = selectedField.Width.Equals(0) ? 15 : selectedField.Width;
                    selectedField.OutputFieldName = item;
                    selectedField.Formula = item;
                    selectedField.tablePrefix = tableName;


                    dynamic addNewLayoutFields = null;
                    if (isCampaign)
                    {
                        addNewLayoutFields = new CampaignExportLayout();
                        addNewLayoutFields.OrderId = campaignId;
                    }
                    else
                    {
                        addNewLayoutFields = new ExportLayoutDetail();
                        addNewLayoutFields.ExportLayoutId = exportLayoutId;
                        addNewLayoutFields.ctabledescription = tableDescription;
                        addNewLayoutFields.cFieldDescription = selectedField.OutputFieldName;
                    }


                    addNewLayoutFields.Id = selectedField.ID;
                    addNewLayoutFields.cFieldName = _buildTableLayoutRepository.GetFieldName(item, tableId).fieldName ?? string.Empty;
                    addNewLayoutFields.iExportOrder = selectedField.Order;
                    addNewLayoutFields.cCalculation = selectedField.Formula;
                    addNewLayoutFields.dCreatedDate = DateTime.Now;
                    addNewLayoutFields.cCreatedBy = _mySession.IDMSUserName;
                    addNewLayoutFields.dModifiedDate = null;
                    addNewLayoutFields.cModifiedBy = null;
                    addNewLayoutFields.iWidth = selectedField.Width;
                    addNewLayoutFields.cOutputFieldName = selectedField.OutputFieldName;
                    addNewLayoutFields.cTableNamePrefix = tableName.ToString();
                    switch (item)
                    {
                        case "IDMS #":
                            if (!isCampaign)
                            {
                                addNewLayoutFields.ctabledescription = "";
                                addNewLayoutFields.cFieldDescription = "";
                            }
                            addNewLayoutFields.cFieldName = "tblSegment.IDMSNumber";
                            addNewLayoutFields.cTableNamePrefix = "";
                            addNewLayoutFields.cCalculation = "";
                            addNewLayoutFields.iWidth = 15;
                            selectedField.tablePrefix = "";
                            selectedField.Width = 15;
                            selectedField.Formula = "";
                            selectedField.fieldName = "tblSegment.IDMSNumber";
                            break;
                        case "Key Code 1":
                            if (!isCampaign)
                            {
                                addNewLayoutFields.ctabledescription = "";
                                addNewLayoutFields.cFieldDescription = "";
                            }
                            addNewLayoutFields.cFieldName = "tblSegment.cKeyCode1";
                            addNewLayoutFields.cTableNamePrefix = "";
                            addNewLayoutFields.cCalculation = "";
                            addNewLayoutFields.iWidth = 15;
                            selectedField.tablePrefix = "";
                            selectedField.Width = 15;
                            selectedField.Formula = "";
                            selectedField.fieldName = "tblSegment.cKeyCode1";
                            break;
                        case "Key Code 2":
                            if (!isCampaign)
                            {
                                addNewLayoutFields.ctabledescription = "";
                                addNewLayoutFields.cFieldDescription = "";
                            }
                            addNewLayoutFields.cFieldName = "tblSegment.cKeyCode2";
                            addNewLayoutFields.cTableNamePrefix = "";
                            addNewLayoutFields.cCalculation = "";
                            addNewLayoutFields.iWidth = 15;
                            selectedField.tablePrefix = "";
                            selectedField.Width = 15;
                            selectedField.Formula = "";
                            selectedField.fieldName = "tblSegment.cKeyCode2";
                            break;
                        case "Distance":
                            if (!isCampaign)
                            {
                                addNewLayoutFields.ctabledescription = "";
                                addNewLayoutFields.cFieldDescription = "";
                            }
                            addNewLayoutFields.cFieldName = "tblSegment.distance";
                            addNewLayoutFields.cTableNamePrefix = "";
                            addNewLayoutFields.cCalculation = "";
                            addNewLayoutFields.iWidth = 15;
                            selectedField.tablePrefix = "";
                            selectedField.Width = 15;
                            selectedField.Formula = "";
                            selectedField.fieldName = "tblSegment.distance";
                            break;
                        case "SelectedSIC":
                            if (!isCampaign)
                            {
                                addNewLayoutFields.ctabledescription = "";
                                addNewLayoutFields.cFieldDescription = "";
                            }
                            addNewLayoutFields.cFieldName = "tblSegment.SpecialSIC";
                            addNewLayoutFields.cTableNamePrefix = "";
                            addNewLayoutFields.cCalculation = "";
                            addNewLayoutFields.iWidth = 15;
                            selectedField.tablePrefix = "";
                            selectedField.Width = 15;
                            selectedField.Formula = "";
                            selectedField.fieldName = "tblSegment.SpecialSIC";
                            break;
                        case "SIC Description":
                            if (!isCampaign)
                            {
                                addNewLayoutFields.ctabledescription = "";
                                addNewLayoutFields.cFieldDescription = "";
                            }
                            addNewLayoutFields.cFieldName = "tblSegment.SICDescription";
                            addNewLayoutFields.cTableNamePrefix = "";
                            addNewLayoutFields.cCalculation = "";
                            addNewLayoutFields.iWidth = 15;
                            selectedField.tablePrefix = "";
                            selectedField.Width = 15;
                            selectedField.Formula = "";
                            selectedField.fieldName = "tblSegment.SICDescription";
                            break;
                        case "Custom...":
                            if (!isCampaign)
                            {
                                addNewLayoutFields.ctabledescription = "";
                                addNewLayoutFields.cFieldDescription = "Custom";
                            }
                            addNewLayoutFields.iIsCalculatedField = true;
                            addNewLayoutFields.cFieldName = "";
                            addNewLayoutFields.cTableNamePrefix = "";
                            addNewLayoutFields.cCalculation = "SPACE(1)";
                            addNewLayoutFields.iWidth = 15;
                            addNewLayoutFields.cOutputFieldName = "Custom";
                            selectedField.iIsCalculatedField = true;
                            selectedField.tablePrefix = "";
                            selectedField.Width = 15;
                            selectedField.Formula = "SPACE(1)";
                            selectedField.OutputFieldName = "Custom";
                            selectedField.fieldName = "";
                            break;
                        default: break;
                    }
                    listSelectedFields.Add(selectedField);
                    if (isCampaign)
                    {
                        _exportLayoutRepository.InsertAsync(addNewLayoutFields);
                        UpdateCampaignStatus(campaignId, campaignStatus);
                    }
                    else
                        _maintainanceExportLayoutDetailRepository.InsertAsync(addNewLayoutFields);
                }
                return listSelectedFields;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void UpdateOrdersAfterDeletion(int campaignId, bool isCampaign, int exportLayoutId)
        {
            dynamic listOfSelectedFields = null;
            if (isCampaign)
            {
                listOfSelectedFields = _exportLayoutRepository.GetAll().Where(x => x.OrderId == campaignId).OrderBy(x => x.iExportOrder);
            }
            else
            {
                listOfSelectedFields = _maintainanceExportLayoutDetailRepository.GetAll().Where(x => x.ExportLayoutId == exportLayoutId).OrderBy(x => x.iExportOrder);
            }
            var newOrder = 0;
            foreach (var item in listOfSelectedFields)
            {
                newOrder++;
                item.iExportOrder = newOrder;
                if (isCampaign)
                    _exportLayoutRepository.UpdateAsync(item);
                else
                    _maintainanceExportLayoutDetailRepository.UpdateAsync(item);

            }
        }
        [HttpPost]
        public void DeleteExportLayoutRecord(List<int> ids, int campaignId, bool isCampaign, int exportLayoutId, int campaignStatus)
        {
            try
            {
                if (isCampaign)
                {
                    foreach (var id in ids)
                    {
                        _exportLayoutRepository.DeleteAsync(id);
                    }
                    UpdateCampaignStatus(campaignId, campaignStatus);
                }
                else
                {
                    foreach (var id in ids)
                    {
                        _maintainanceExportLayoutDetailRepository.DeleteAsync(id);
                    }
                }
                CurrentUnitOfWork.SaveChanges();
                UpdateOrdersAfterDeletion(campaignId, isCampaign, exportLayoutId);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private Tuple<string, string> GetTableNameAndDescription(GetExportLayoutSelectedFieldsDto record, int campaignId, int maintainanceBuildId, bool isCampaign, int databaseId, int tableId)
        {
            var tableDescription = string.Empty;
            var tableName = string.Empty;
            var tableList = GetTableDescriptionDropDownValues(campaignId, maintainanceBuildId, isCampaign, databaseId);
            if (tableId > 0)
            {
                foreach (var item in tableList)
                {
                    if (Convert.ToInt32(item.Value) == tableId)
                    {
                        var tableEntity = _buildRepository.Get(tableId).LK_TableType;
                        tableDescription = item.Label.ToString().Substring(0, item.Label.IndexOf("("));
                        if (tableEntity.Equals("M"))
                        {

                            tableName = "MainTable";
                        }
                        else
                            tableName = item.Label.ToString().Split('(').Last().TrimEnd(')');
                        break;
                    }

                }
            }
            else
            {
                if (!string.IsNullOrEmpty(record.tableDescription))
                {
                    tableDescription = record.tableDescription.ToString().Substring(0, record.tableDescription.IndexOf("("));
                    tableName = record.tableDescription.ToString().Split('(').Last().TrimEnd(')');
                    if (tableName.Equals("tblMain"))
                    {
                        tableName = "MainTable";
                    }
                }

            }
            return new Tuple<string, string>(tableDescription, tableName);
        }


        public void UpdateExportLayoutRecords(GetExportLayoutSelectedFieldsDto record, bool isCampaign, int campaignId, int campaignStatus, int maintainanceBuildId, int databaseId, int tableId)
        {
            var tableDescription = string.Empty;
            var tableName = string.Empty;
            try
            {
                dynamic editExportLayout = null;
                if (isCampaign)
                {
                    editExportLayout = _exportLayoutRepository.Get(record.ID);
                    UpdateCampaignStatus(campaignId, campaignStatus);

                    var tabledetails = GetTableNameAndDescription(record, campaignId, maintainanceBuildId, isCampaign, databaseId, tableId);
                    tableDescription = tabledetails.Item1;
                    tableName = tabledetails.Item2;
                }
                else
                {
                    editExportLayout = _maintainanceExportLayoutDetailRepository.Get(record.ID);
                    var tabledetails = GetTableNameAndDescription(record, campaignId, maintainanceBuildId, isCampaign, databaseId, tableId);
                    tableDescription = tabledetails.Item1;
                    tableName = tabledetails.Item2;
                }
                editExportLayout.iExportOrder = record.Order;
                editExportLayout.cOutputFieldName = record.OutputFieldName;
                editExportLayout.cCalculation = record.Formula;
                editExportLayout.cFieldName = record.fieldName;
                editExportLayout.iWidth = record.Width;
                editExportLayout.cModifiedBy = _mySession.IDMSUserName;
                if (!isCampaign)
                {
                    editExportLayout.ctabledescription = tableDescription;
                }
                editExportLayout.cTableNamePrefix = tableName;
                editExportLayout.dModifiedDate = DateTime.Now;
                editExportLayout.iIsCalculatedField = record.iIsCalculatedField;
                if (editExportLayout.cOutputFieldName == "Custom")
                {
                    editExportLayout.iIsCalculatedField = true;
                }
                if (isCampaign)
                    _exportLayoutRepository.UpdateAsync(editExportLayout);
                else
                    _maintainanceExportLayoutDetailRepository.UpdateAsync(editExportLayout);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        public List<GetExportLayoutSelectedFieldsDto> CheckForFixedAndCuStomFields(List<GetExportLayoutSelectedFieldsDto> exportLayout)
        {
            foreach (var item in exportLayout)
            {
                item.isFixedFields = CheckFields(item.fieldName.Trim());
                if (item.isFixedFields)
                {
                    item.isFormulaEnabled = false;
                    if (item.fieldName.Trim() == "")
                    {
                        item.iIsCalculatedField = true;
                    }

                }
                if (item.iIsCalculatedField)
                {
                    item.isFormulaEnabled = true;

                }
            }
            return exportLayout;
        }

        public List<GetExportLayoutSelectedFieldsDto> GetExportLayoutSelectedFields(int campaignId, bool isCampaign, int exportLayoutId, int maintainanceBuildId)
        {
            try
            {
                var exportLayout = new List<GetExportLayoutSelectedFieldsDto>();
                if (isCampaign)
                {
                    var buildId = _customCampaignRepository.Get(campaignId).BuildID;
                    exportLayout = _customCampaignRepository.GetExportLayoutSelectedFields(buildId, campaignId);
                }
                else
                {
                    exportLayout = _maintainanceExportLayoutRepository.GetExportLayoutSelectedFields(exportLayoutId, maintainanceBuildId);
                }
                if (exportLayout != null && exportLayout.Count > 0)
                    exportLayout = CheckForFixedAndCuStomFields(exportLayout);
                return exportLayout;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private bool CheckFields(string fieldName)
        {
            switch (fieldName.ToUpper())
            {
                case "": return true;
                case "TBLSEGMENT.IDMSNUMBER":
                case "TBLSEGMENT.CKEYCODE1":
                case "TBLSEGMENT.CKEYCODE2":
                case "TBLSEGMENT.DISTANCE":
                case "TBLSEGMENT.SPECIALSIC":
                case "TBLSEGMENT.SICDESCRIPTION": return true;
                default: return false;
            }
        }

        public void ReorderExportLayoutOrderId(int id, int orderId, int campaignId, bool isCampaign)
        {
            try
            {
                var modifiedBy = _mySession.IDMSUserName;
                if (isCampaign)
                    _exportLayoutRepository.ReorderExportLayoutOrderId(id, orderId, modifiedBy);
                else
                    _maintainanceExportLayoutRepository.ReorderExportLayoutOrderId(id, orderId, modifiedBy);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private void UpdateCampaignStatus(int campaignId, int campaignStatus)
        {
            switch ((CampaignStatus)campaignStatus)
            {
                case CampaignStatus.OutputFailed:
                case CampaignStatus.OutputCompleted:
                    _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.OrderCompleted, _mySession.IDMSUserName);
                    break;
                case CampaignStatus.OrderFailed:
                    _orderStatusManager.UpdateOrderStatus(campaignId, CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                    break;
                default:
                    break;
            }
        }


        #endregion

        #region Maintainance Export Layout
        public async Task<PagedResultDto<GetExportLayoutForViewDto>> GetAllExportLayoutsList(string outputCase, string fieldDescription, int databaseId, GetExportLayoutFilters filters)
        {
            try
            {
                filters.Filter = string.IsNullOrEmpty(filters.Filter) ? string.Empty : filters.Filter;
                var databaseIds = _customCampaignRepository.GetDatabaseIdByUserID(_mySession.IDMSUserId);
                var query = GetAllExportLayouts(filters, databaseId, _mySession.IDMSUserId, fieldDescription, outputCase);
                return await _maintainanceExportLayoutRepository.GetAllExportLayoutsList(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }



        public List<DropdownOutputDto> GetGroupNames(int databaseId, int isActive)
        {
            try
            {
                var query = GetGroupData(_mySession.IDMSUserId, databaseId, isActive);
                return _maintainanceExportLayoutRepository.GetGroupDataByDatabaseAndUserID(query.Item1, query.Item2).OrderBy(x => x.Label).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<ExportLayoutFieldsDto> GetExportLayoutFields(int iExportLOID)
        {
            return _maintainanceExportLayoutRepository.GetExportLayoutFields(iExportLOID);
        }


        public void AddExportLayoutRecord(CreateOrEditExportLayoutDto record)
        {
            try
            {
                var exportLayoutRecord = new ExportLayout()
                {
                    DatabaseId = record.DatabaseId,
                    cDescription = record.cDescription,
                    iHasPhone = record.iHasPhone,
                    iIsActive = true,
                    dCreatedDate = DateTime.Now,
                    cCreatedBy = _mySession.IDMSUserName,
                    GroupID = record.GroupID,
                    iHasKeyCode = record.iHasKeyCode,
                    cOutputCase = record.cOutputCaseCode
                };
                _maintainanceExportLayoutRepository.InsertAsync(exportLayoutRecord);

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void UpdateMaitainanceExportLayoutRecords(GetExportLayoutForViewDto record)
        {
            try
            {
                var exportLayoutRecord = _maintainanceExportLayoutRepository.Get(record.ID);
                exportLayoutRecord.DatabaseId = record.DatabaseId;
                exportLayoutRecord.cDescription = record.cDescription;
                exportLayoutRecord.iHasPhone = record.iHasPhone;
                exportLayoutRecord.iIsActive = true;
                exportLayoutRecord.dModifiedDate = DateTime.Now;
                exportLayoutRecord.cModifiedBy = _mySession.IDMSUserName;
                exportLayoutRecord.GroupID = record.GroupID;
                exportLayoutRecord.iHasKeyCode = record.iHasKeyCode;
                if (!string.IsNullOrEmpty(record.cOutputCaseCode))
                {
                    exportLayoutRecord.cOutputCase = record.cOutputCaseCode;
                }
                _maintainanceExportLayoutRepository.UpdateAsync(exportLayoutRecord);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void DeleteMaintainanceExportLayoutRecord(int id)
        {
            try
            {
                var exportLayoutRecord = _maintainanceExportLayoutRepository.Get(id);
                exportLayoutRecord.iIsActive = false;
                exportLayoutRecord.dModifiedDate = DateTime.Now;
                exportLayoutRecord.cModifiedBy = _mySession.IDMSUserName;
                _maintainanceExportLayoutRepository.UpdateAsync(exportLayoutRecord);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void CopyExportLayout(GetExportLayoutForViewDto record)
        {
            try
            {
                record.cCreatedBy = _mySession.IDMSUserName;
                _maintainanceExportLayoutRepository.CopyExportLayout(record);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public List<DropdownOutputDto> GetBuildsByDatabase(int databaseId)
        {
            try
            {
                return _buildsRepository.GetAll().Where(x => x.DatabaseId == databaseId && x.iIsReadyToUse).OrderBy(a => a.cBuild).Select(x => new DropdownOutputDto { Label = x.Id + " : " + x.cDescription, Value = x.Id }).OrderByDescending(x => x.Value).ToList();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public FileDto DownloadExportLayoutExcel(int exportLayoutId, int databaseId, int buildId, int isActive)
        {
            try
            {
                var exportLayout = _maintainanceExportLayoutRepository.Get(exportLayoutId);
                var exportLayoutDetail = _maintainanceExportLayoutDetailRepository.GetAll().Where(x => x.ExportLayoutId == exportLayoutId).OrderBy(x=> x.iExportOrder);
                var groupName = GetGroupNames(databaseId, isActive).FirstOrDefault(x => Convert.ToInt32(x.Value) == exportLayout.GroupID).Label;
                var database = _userCache.GetDropdownOptions(_mySession.IDMSUserId, UserDropdown.Databases).FirstOrDefault(x => Convert.ToInt32(x.Value) == databaseId).Label;
                var databaseName = $"Database Name:{database.Substring(database.IndexOf(':') + 1)}";
                var buildID = $"Build:{buildId}";
                var excelExportLayout = new List<ExportLayoutTemplateDto>();
                excelExportLayout.Add(new ExportLayoutTemplateDto { Description = exportLayout.cDescription, GroupName = groupName, Telemarketing = exportLayout.iHasPhone ? "Yes" : "No", OutputCase = exportLayout.cOutputCase });

                var excelExportLayoutDetails = new List<ExportLayoutTemplateDto>();
                foreach (var item in exportLayoutDetail)
                {
                    excelExportLayoutDetails.Add(new ExportLayoutTemplateDto
                    {
                        Order = item.iExportOrder.ToString(),
                        OutputFieldName = item.cOutputFieldName,
                        Formula = item.cCalculation,
                        Width = item.iWidth.ToString(),
                        TableName = item.cTableNamePrefix,
                        TableDescription = item.ctabledescription

                    });
                }
                return _layoutExcelExporter.ExportToFile(excelExportLayoutDetails, excelExportLayout, databaseName, buildID);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region Copy Export Layout
        public PagedResultDto<GetCopyAllExportLayoutForViewDto> GetAllExportLayout(GetAllExportLayoutForCopyDto input)
        {
            try
            {
                var query = GetExportLayoutForCopyQuery(input, _mySession.IDMSUserId);
                return _maintainanceExportLayoutRepository.GetExportLayoutForCopy(query);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void CopyAllExportLayout(CopyAllExportLayoutDto input)
        {
            try
            {
                if (input.GroupFromId == input.GroupToId)
                    throw new UserFriendlyException(L("GroupNameSameError"));

                var layoutIds = BuildLayoutIdXMLString(input.Layouts);

                var IntiatedBy = _mySession.IDMSUserName;
                var ErrorNo = _maintainanceExportLayoutRepository.CopyAllExportLayout(input, IntiatedBy, layoutIds);
                if (ErrorNo > 0)
                {
                    throw new UserFriendlyException(L("CopyExportLayoutError", ErrorNo.ToString()));
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private static string BuildLayoutIdXMLString(List<GetCopyAllExportLayoutForViewDto> Layouts)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("<root>");
                foreach (var layout in Layouts)
                {
                    sb.AppendFormat($@"<r><LID>{layout.Id.ToString()}</LID></r>");
                }
                sb.Append("</root>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion


        #region Import Export Layout

        public void GetFieldsCounts(int layoutId)
        {
            try
            {
                var fieldCount = _maintainanceExportLayoutDetailRepository.GetAll()
                    .Count(detail => detail.ExportLayoutId == layoutId);
                if (fieldCount > 0)
                    throw new UserFriendlyException(L("LayoutPresentError"));
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public FileDto GetTemplatePath(int databaseId)
        {
            try
            {
                var fileName = "Export Layout Upload Template.xlsx";
                string directoryPath;
                var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseId);
                if (awsFlag)
                {
                    directoryPath = _idmsConfigurationCache.GetConfigurationValue("TemplatePathAWS", 0).cValue;
                    directoryPath = directoryPath.Trim('/') + "/";
                }
                else
                {
                    directoryPath = _idmsConfigurationCache.GetConfigurationValue("TemplatePath", 0).cValue;
                    directoryPath = directoryPath.TrimEnd('\\') + @"\";
                }
                var fullPath = directoryPath + fileName;
                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
                return new FileDto(fullPath, contentType, fileName, isAWS: awsFlag);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public void ImportLayout(ImportLayoutDto input, int maintainanceBuildId, int campaignId, bool isCampaign, int databaseId)
        {
            try
            {
                var layoutTable = ValidateFile(input, maintainanceBuildId,campaignId, isCampaign, databaseId);
                SaveToLayout(layoutTable, input.LayoutId);
                File.Delete(input.Path);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        private void SaveToLayout(DataTable layoutTable, int layoutId)
        {
            try
            {
                for (int counter = 0; counter < layoutTable.Rows.Count; counter++)
                {
                    var layoutDetail = new ExportLayoutDetail();
                    string outputFieldName = layoutTable.Rows[counter]["Output Field Name"].ToString().Trim();
                    string fieldDescription = layoutTable.Rows[counter]["Field Description"].ToString().Trim();
                    string fieldName = layoutTable.Rows[counter]["Field Name"].ToString().Trim();
                    string tableNamePrefix = layoutTable.Rows[counter]["Table Name Prefix"].ToString().Trim();
                    string tableDescription = layoutTable.Rows[counter]["Table Description"].ToString().Trim();
                    string calCulation = layoutTable.Rows[counter]["Calculation"].ToString().Trim();

                    var sWidth = layoutTable.Rows[counter]["Width"].ToString().Trim();
                    int.TryParse(sWidth, out int iWidth);
                    layoutDetail.iWidth = iWidth;
                    layoutDetail.cTableNamePrefix = tableNamePrefix;
                    layoutDetail.ctabledescription = tableDescription;
                    layoutDetail.cFieldDescription = fieldDescription;
                    layoutDetail.cOutputFieldName = outputFieldName;
                    layoutDetail.cFieldName = fieldName;
                    layoutDetail.cCreatedBy = _mySession.IDMSUserName;
                    layoutDetail.dCreatedDate = DateTime.Now;

                    layoutDetail.ExportLayoutId = layoutId;
                    layoutDetail.iExportOrder = counter + 1;
                    if (string.IsNullOrEmpty(calCulation))
                    {
                        layoutDetail.iIsCalculatedField = false;
                        layoutDetail.cCalculation = string.Empty;
                    }
                    else  //Custom Field
                    {
                        layoutDetail.cFieldDescription = string.Empty;
                        layoutDetail.cFieldName = string.Empty;
                        layoutDetail.cTableNamePrefix = string.Empty;
                        layoutDetail.ctabledescription = string.Empty;
                        layoutDetail.cCalculation = calCulation;
                        layoutDetail.iIsCalculatedField = true;
                    }
                    var tablePrefix = layoutDetail.cTableNamePrefix;
                    var tableName = layoutDetail.ctabledescription;

                    _maintainanceExportLayoutDetailRepository.Insert(layoutDetail);
                }
               
                CurrentUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public DataTable ValidateFile(ImportLayoutDto input , int maintainanceBuildId, int campaignId,bool isCampaign, int databaseId)
        {
            try
            {
                var excelEngine = new ExcelEngine();
                var application = excelEngine.Excel;
                var errorLogs = new List<ErrorMsg>();
                var fieldNameList = new List<string>();
                using (var inputStream = new FileStream(input.Path, FileMode.Open))
                {
                    var workbook = application.Workbooks.Open(inputStream);
                    IWorksheet sheet = workbook.Worksheets[0];
                    int columnLength = sheet.Columns.Length;
                    bool isColumnMatch = false;

                    if (sheet.Columns.Length == 0 || sheet.Columns.Length != 7)
                        throw new UserFriendlyException(L("MissingColumnsError"));

                    for (int k = 0; k < sheet.Columns.Length; k++)
                    {
                        if (
                            sheet.Rows[0].Columns[k].Value.ToUpper().Trim() == "Output Field Name".ToUpper() ||
                            sheet.Rows[0].Columns[k].Value.ToUpper().Trim() == "Field Description".ToUpper() ||
                            sheet.Rows[0].Columns[k].Value.ToUpper().Trim() == "Field Name".ToUpper() ||
                            sheet.Rows[0].Columns[k].Value.ToUpper().Trim() == "Calculation".ToUpper() ||
                            sheet.Rows[0].Columns[k].Value.ToUpper().Trim() == "Width".ToUpper() ||
                            sheet.Rows[0].Columns[k].Value.ToUpper().Trim() == "Table Name Prefix".ToUpper() ||
                            sheet.Rows[0].Columns[k].Value.ToUpper().Trim() == "Table Description".ToUpper())
                        {
                            isColumnMatch = true;
                        }
                        else
                        {
                            isColumnMatch = false;
                            break;
                        }
                    }

                    if (!isColumnMatch)
                        throw new UserFriendlyException(L("MissingColumnsError"));

                    var layoutTable = new DataTable();
                    layoutTable = sheet.ExportDataTable(sheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);

                    if (layoutTable.Rows.Count > 1000)
                    {
                        throw new UserFriendlyException(L("LayoutMaxRowLimit"));
                    }

                    //validate the column range and Empty value
                    for (int miCounter = 0; miCounter < layoutTable.Rows.Count && errorLogs.Count < 5; miCounter++)
                    {
                        var outputFieldName = layoutTable.Rows[miCounter]["Output Field Name"].ToString().Trim();
                        var fieldDescription = layoutTable.Rows[miCounter]["Field Description"].ToString().Trim();
                        var fieldName = layoutTable.Rows[miCounter]["Field Name"].ToString().Trim();
                        var tableNamePrefix = layoutTable.Rows[miCounter]["Table Name Prefix"].ToString().Trim();
                        var tableDescription = layoutTable.Rows[miCounter]["Table Description"].ToString().Trim();
                        var calCulation = layoutTable.Rows[miCounter]["Calculation"].ToString().Trim();
                        var width = layoutTable.Rows[miCounter]["Width"].ToString().Trim();

                        if (outputFieldName == string.Empty)
                            errorLogs.Add(new ErrorMsg((miCounter).ToString(), L("LayoutOutputFieldEmpty")));

                        if (width == string.Empty || width == "0")
                            errorLogs.Add(new ErrorMsg((miCounter).ToString(), L("LayoutWidthInvalid")));
                        if (width != string.Empty)
                        {
                            if (!int.TryParse(width, out int num))
                                errorLogs.Add(new ErrorMsg((miCounter).ToString(), L("LayoutWidthInvalid")));
                            else if (Convert.ToInt32(layoutTable.Rows[miCounter]["Width"].ToString().Trim()) < 0)
                                errorLogs.Add(new ErrorMsg((miCounter).ToString(), L("LayoutWidthInvalid")));
                        }

                        if (calCulation == string.Empty)
                            if (fieldDescription == string.Empty || fieldName == string.Empty || tableDescription == string.Empty ||
                                tableNamePrefix == string.Empty)
                                errorLogs.Add(new ErrorMsg((miCounter).ToString(), L("LayoutInvalidData")));
                    }


                    for (int miCounter = 0; miCounter < layoutTable.Rows.Count; miCounter++)
                    {
                        var buildId = 0;
                        if (!isCampaign)
                        {
                            buildId = maintainanceBuildId;
                        }
                        else
                        {
                            buildId = _customCampaignRepository.Get(campaignId).BuildID;
                        }
                        var tableDescription = layoutTable.Rows[miCounter]["Table Description"].ToString().Trim();
                        var data = _buildRepository.CheckTableDecriptionOfExcelSheet(buildId, tableDescription);
                        var fieldName = layoutTable.Rows[miCounter]["Field Name"].ToString().Trim();

                        var dataOfBuildTableLayout = _buildTableLayoutRepository.GetExportableField(fieldName, data.Id);

                        if (dataOfBuildTableLayout.iAllowExport == false)
                        {
                            fieldNameList.Add(fieldName);

                        }
                    }

                    if (fieldNameList.Count > 0)
                    {
                        string msg = string.Join(",", fieldNameList);
                        string message = msg + "  " + L("FieldNotExportable");
                        throw new UserFriendlyException(message);
                    }


                    if (errorLogs.Count > 0)
                    {
                        string message = ErrorMessageFormatter.GetNumberedMessage(errorLogs);
                        throw new UserFriendlyException(message);
                    }
                    return layoutTable;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        #endregion

        private Tuple<string, string, List<SqlParameter>> GetAllExportLayouts(GetExportLayoutFilters filters, int databaseId, int userId, string fieldDescription, string outputCase)
        {


            if (!string.IsNullOrEmpty(filters.Filter))
                filters.Filter = filters.Filter.Trim();

            var isExpLayoutId = Validation.ValidationHelper.IsNumeric(filters.Filter);

            string[] filtersarray = null;
            try
            {
                if (!string.IsNullOrEmpty(filters.Filter))
                    filtersarray = filters.Filter.Split(',');

                var descriptionandGroupnameFilter = $@"AND (E.cDescription LIKE @FilterText OR G.cgroupname LIKE @FilterText)";
                var fieldDescriptionFilter = $@"E.ID IN (select exportlayoutid from tblexportlayoutdetail where cOutPutFieldName like @FieldDescription)";
                var query = new Common.QueryBuilder();
                query.AddSelect("distinct E.ID, E.DatabaseID, E.GroupID, cDescription, G.cgroupname,E.iHasKeyCode,E.iHasPhone,cOutputCase,CASE WHEN cOutputCase = 'U' THEN 'Upper Case' WHEN cOutputCase = 'L' THEN 'Lower Case' WHEN cOutputCase = 'P' THEN 'Proper Case' WHEN cOutputCase = 'D' THEN 'Default' END AS cOutputCaseLabel,E.dCreatedDate,E.dModifiedDate, E.cCreatedBy, E.cModifiedBy");
                query.AddFrom("tblExportLayout", "E");
                query.AddJoin("tblGroup", "G", "GroupID", "E", "INNER JOIN", "ID").And("G.DatabaseID", "EQUALTO", "E.DatabaseID");
                query.AddJoin("tblUserGroup", "UG", "ID", "G", "INNER JOIN", "GroupID");

                query.AddWhere("", "E.iIsActive", "EQUALTO", "1");
                query.AddWhere("AND", "E.DatabaseID ", "IN", databaseId.ToString());
                query.AddWhere("AND", "UG.UserID", "EQUALTO", userId.ToString());
                if (isExpLayoutId)
                {
                    query.AddWhere("AND", "E.ID", "IN", filtersarray);
                }
                else
                {
                    query.AddWhereString(descriptionandGroupnameFilter);
                }
                if (!string.IsNullOrEmpty(fieldDescription))
                    query.AddWhereString($"AND ({fieldDescriptionFilter})");

                if (!string.IsNullOrEmpty(outputCase))
                    query.AddWhere("AND", "E.cOutputCase", "EQUALTO", outputCase);

                query.AddSort(filters.Sorting ?? "E.ID DESC");
                query.AddOffset($"OFFSET {filters.SkipCount} ROWS FETCH NEXT {filters.MaxResultCount} ROWS ONLY;");
                (string sqlSelect, List<SqlParameter> sqlParams) = query.Build();
                sqlParams.Add(new SqlParameter("@FilterText", $"%{filters.Filter}%"));
                sqlParams.Add(new SqlParameter("@FieldDescription", $"%{fieldDescription}%"));
                var sqlCount = query.BuildCount().Item1;
                return new Tuple<string, string, List<SqlParameter>>(sqlSelect.ToString(), sqlCount.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private Tuple<string, List<SqlParameter>> GetGroupData(int iUserID, int iDatabaseID, int ShowOnlyActiveGroup)

        {
            var query = new Common.QueryBuilder();
            query.AddSelect("G.cGroupName, G.ID, G.iIsActive");
            query.AddFrom("tblGroup", "G");
            query.AddJoin("tblUserGroup", "UG", "ID", "G", "INNER JOIN", "GroupID");
            query.AddWhere("", "UG.UserID", "EQUALTO", iUserID.ToString());
            query.AddWhere("AND", "G.DatabaseID", "EQUALTO", iDatabaseID.ToString());
            query.AddWhere("AND", "G.iIsActive", "EQUALTO", ShowOnlyActiveGroup.ToString());
            (string sql, List<SqlParameter> sqlParams) = query.Build();
            return new Tuple<string, List<SqlParameter>>(sql.ToString(), sqlParams);


        }

        private Tuple<string, string, List<SqlParameter>> GetExportLayoutForCopyQuery(GetAllExportLayoutForCopyDto input, int UserId)
        {
            var query = new Common.QueryBuilder();
            query.AddSelect("E.ID, cDescription");
            query.AddFrom("tblExportLayout", "E");
            query.AddJoin("tblGroup", "G", "GroupID", "E", "INNER JOIN", "ID").And("G.DatabaseID", "EQUALTO", "E.DatabaseID");
            query.AddJoin("tblUserGroup", "UG", "ID", "G", "INNER JOIN", "GroupID");
            query.AddWhere("", "E.iIsActive", "EQUALTO", "1");
            query.AddWhere("AND", "E.DatabaseID", "EQUALTO", input.DatabaseId.ToString());
            query.AddWhere("AND", "G.Id", "EQUALTO", input.GroupId.ToString());
            query.AddWhere("AND", "UG.UserID", "EQUALTO", UserId.ToString());

            query.AddSort(input.Sorting ?? "'cDescription' ASC");
            query.AddDistinct();
            (string sql, List<SqlParameter> sqlParams) = query.Build();
            var sqlCount = query.BuildCount().Item1;
            return new Tuple<string, string, List<SqlParameter>>(sql.ToString(), sqlCount.ToString(), sqlParams);
        }
    }
}