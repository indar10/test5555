using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Authorization;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.UI;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.Segments;
using Infogroup.IDMS.Sessions;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.UserDivisions;
using Infogroup.IDMS.CampaignCASApprovals;
using Infogroup.IDMS.CampaignCASApprovals.Dtos;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.CampaignMaxPers;
using Infogroup.IDMS.Decoys;
using Infogroup.IDMS.Decoys.Dtos;
using Infogroup.IDMS.CampaignDecoys;
using Infogroup.IDMS.CampaignXTabReports;
using System.Collections.Generic;
using Infogroup.IDMS.CampaignFTPs;
using Infogroup.IDMS.Campaigns.Exporting;
using Infogroup.IDMS.CampaignMultiColumnReports;
using Infogroup.IDMS.CampaignAttachments;
using Infogroup.IDMS.CampaignExportLayouts;
using Infogroup.IDMS.Builds.Dtos;
using Infogroup.IDMS.UserDatabases;
using Infogroup.IDMS.DivisionMailers;
using Infogroup.IDMS.DivisionBrokers;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.Mailers;
using Infogroup.IDMS.UserGroups;
using Infogroup.IDMS.Offers;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.GroupBrokers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Infogroup.IDMS.Configuration;
using Infogroup.IDMS.AutoSuppresses;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.OrderExportParts;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.CampaignBillings;
using Infogroup.IDMS.Common;
using Infogroup.IDMS.ShortSearch;
using Infogroup.IDMS.UserDatabaseMailers.Dtos;
using Infogroup.IDMS.Caching;
using Infogroup.IDMS.SegmentSelections;
using System.Data.SqlClient;
using Infogroup.IDMS.Shared;
using Castle.Core.Logging;
using System.Diagnostics;
using Infogroup.IDMS.SubSelects;
using Infogroup.IDMS.SegmentLists;
using Infogroup.IDMS.CampaignMaxPers.Dtos;

namespace Infogroup.IDMS.Campaigns
{
    [AbpAuthorize(AppPermissions.Pages_Campaigns,AppPermissions.Pages_FastCount)]
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        private readonly IRepository<Campaign, int> _campaignRepository;
        private readonly IRepository<CampaignFTP, int> _campaignFTPRepository;
        private readonly IRepository<IDMSUser, int> _userRepository;
        private readonly IRepository<UserDivision> _UserDivisionRepository;
        private readonly IRepository<UserDatabase> _userDatabaseRepository;
        private readonly IRepository<Segment, int> _segmentRepository;
        private readonly IRepository<OrderStatus> _orderStatusRepository;
        private readonly IRepository<CampaignCASApproval, int> _campaignCASApprovalRepository;
        private readonly IRepository<Lookup, int> _lookupRepository;
        private readonly IRepository<CampaignMaxPer, int> _campaignMaxPerRepository;
        private readonly IRepository<CampaignDecoy, int> _campaignDecoyRepository;
        private readonly IRepository<UserDivision> _userDivisionRepository;
        private readonly IRepository<DivisionMailer> _divisionMailerRepository;
        private readonly IRepository<DivisionBroker> _divisionBrokerRepository;
        private readonly IRepository<Mailer> _mailerRepository;
        private readonly IRepository<Offer> _offerRepository;
        private readonly IRepository<UserGroup> _userGroupRepository;
        private readonly IRepository<GroupBroker> _groupBrokerRepository;
        private readonly IRepository<CampaignExportLayout, int> _campaignExportLayoutRepository;
        private readonly IRepository<SubSelect , int> _subSelectRepository;
        private readonly IRepository<SegmentList , int> _segmentListRepository;
        private readonly ISharedRepository _sharedRepository;

        private readonly IBuildRepository _buildRepository;
        private readonly ICampaignRepository _customCampaignRepository;
        private readonly IBuildTableLayoutRepository _buildTableLayoutRepository;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly ICampaignCASApprovalRepository _customCASApprovalRepository;
        private readonly IDecoyRepository _customDecoyRepository;
        private readonly IBuildsAppService _buildAppService;
        private readonly IOrderStatusManager _orderStatusManager;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly ICampaignXTabReportsRepository _customCampaignXtabReportsRepository;
        private readonly ICampaignAttachmentRepository _customCampaignAttachmentsRepository;
        private readonly ICampaignMultiColumnReportRepository _customCampaignMultiDimensionalReportsRepository;
        private readonly ILayoutTemplateExcelExporter _layoutTemplateExcelExporter;
        private readonly IOrderExportPartsAppService _orderExportPartsAppService;

        private readonly AppSession _mySession;
        private readonly CampaignBizness _campaignBizness;
        private readonly CampaignMultiColumnReportBizness _campaignMulticolumnReportBizness;
        private readonly ICampaignMaxPersRepository _customCampaignMaxPerRepository;

        //private readonly string MaxPerDefaultValueConfig = "SegmentLevelMaxPerDefaultValue";
        private readonly string IsDivisionalDBConfig = "UseDivisionalCustomer";
        private readonly string DefaultMailerConfig = "UseDefaultMailer";
        private readonly string DefaultOfferConfig = "UseDefaultOffer";
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IRepository<AutoSuppress, Guid> _autoSuppressRepository;
        private readonly IRepository<BuildTableLayout> _buildtblLayoutRepository;
        private readonly IRepository<BuildTable, int> _buildTableRepository;
        private readonly IRepository<Decoy, int> _decoyRepository;
        private readonly IRepository<CampaignBilling, int> _campaignBillingRepository;
        private readonly Utils _utils;
        private readonly IShortSearch _shortSearch;
        private readonly ISegmentSelectionsAppService _segmentSelectionsAppService;

        private readonly IRedisLookupCache _lookupCache;
        private readonly IRedisIDMSUserCache _userCache;
        private readonly IRedisBuildCache _buildCache;
        private readonly IRedisCacheHelper _redisCacheHelper;
        private readonly IIDMSPermissionChecker _permissionChecker;
        private const string ConditionalWhere = @" AND ((CHARINDEX(@Filter, M.cCompany) > 0)
                                                    OR (CHARINDEX(@Filter, M.cCode) > 0) 
                                                    OR (CHARINDEX(@Filter, CONVERT(VARCHAR(11), M.Id)) > 0))";

        public CampaignsAppService(
            IRepository<UserDatabase> userDatabaseRepository,
            IBuildRepository buildRepository,
            IRepository<DivisionMailer> divisionMailerRepository,
            IRepository<DivisionBroker> divisionBrokerRepository,
            IRepository<Mailer> mailerRepository,
            IRepository<Offer> offerRepository,
            IRepository<UserGroup> userGroupRepository,
            IRepository<GroupBroker> groupBrokerRepository,
            IRepository<Campaign, int> campaignRepository,
            IRepository<IDMSUser, int> userRepository,
            IRepository<UserDivision> UserDivisionRepository,
            IRepository<Segment, int> segmentRepository,
            IRepository<SubSelect , int> subSelectRepository,
            IRepository<SegmentList , int> segmentListRepository,
            ICampaignRepository customCampaignRepository,
            IDatabaseRepository databaseRepository,
            IRepository<OrderStatus> orderStatusRepository,
            IRepository<UserDivision> userDivisionRepository,
            IRedisIDMSConfigurationCache idmsConfigurationCache,
            IBuildsAppService buildAppService,
            IOrderStatusManager orderStatusManager,
            AppSession mySession,
            CampaignBizness campaignBizness,
            CampaignMultiColumnReportBizness campaignMulticolumnReportBizness,
            ICampaignCASApprovalRepository customCASApprovalRepository,
            ISharedRepository sharedRepository,
            IRepository<CampaignCASApproval, int> campaignCASApprovalRepository,
            IRepository<Lookup, int> lookupRepository,
            IRepository<CampaignMaxPer, int> campaignMaxPerRepository,
            IDecoyRepository customDecoyRepository,
            IRepository<CampaignDecoy, int> campaignDecoyRepository,
            ICampaignXTabReportsRepository customCampaignXtabReportsRepository,
            IRepository<CampaignFTP, int> campaignFTPRepository,
            ILayoutTemplateExcelExporter layoutTemplateExcelExporter,
            ICampaignMultiColumnReportRepository customCampaignMultiDimensionalReportsRepository,
            IBuildTableLayoutRepository buildTableLayoutRepository,
            ICampaignAttachmentRepository customCampaignAttachmentsRepository,
            IRepository<CampaignExportLayout, int> campaignExportLayoutRepository,
            ICampaignMaxPersRepository customCampaignMaxPerRepository,
            IHostingEnvironment env,
            IOrderExportPartsAppService orderExportPartsAppService,
            IRepository<AutoSuppress, Guid> autoSuppressRepository,
            IRepository<BuildTableLayout> buildtblLayoutRepository,
            IRepository<BuildTable, int> buildTableRepository,
            IRepository<Decoy, int> decoyRepository,
            IRepository<CampaignBilling, int> campaignBillingRepository,
            IRedisLookupCache lookupCache,
            IRedisIDMSUserCache userCache,
            IRedisBuildCache buildCache,
            IRedisCacheHelper redisCacheHelper,
            Utils utils,
            IShortSearch shortSearch,
            IIDMSPermissionChecker permissionChecker,
            ISegmentSelectionsAppService segmentSelectionsAppService
            )
        {
            _userRepository = userRepository;
            _UserDivisionRepository = UserDivisionRepository;
            _campaignRepository = campaignRepository;
            _segmentRepository = segmentRepository;
            _customCampaignRepository = customCampaignRepository;
            _orderStatusRepository = orderStatusRepository;
            _databaseRepository = databaseRepository;
            _buildAppService = buildAppService;
            _orderStatusManager = orderStatusManager;
            _mySession = mySession;
            _campaignBizness = campaignBizness;
            _idmsConfigurationCache = idmsConfigurationCache;
            _customCASApprovalRepository = customCASApprovalRepository;
            _sharedRepository = sharedRepository;
            _campaignCASApprovalRepository = campaignCASApprovalRepository;
            _lookupRepository = lookupRepository;
            _campaignMaxPerRepository = campaignMaxPerRepository;
            _customDecoyRepository = customDecoyRepository;
            _campaignDecoyRepository = campaignDecoyRepository;
            _customCampaignXtabReportsRepository = customCampaignXtabReportsRepository;
            _campaignFTPRepository = campaignFTPRepository;
            _layoutTemplateExcelExporter = layoutTemplateExcelExporter;
            _customCampaignMultiDimensionalReportsRepository = customCampaignMultiDimensionalReportsRepository;
            _campaignMulticolumnReportBizness = campaignMulticolumnReportBizness;
            _customCampaignAttachmentsRepository = customCampaignAttachmentsRepository;
            _campaignExportLayoutRepository = campaignExportLayoutRepository;
            _userDivisionRepository = userDivisionRepository;
            _userDatabaseRepository = userDatabaseRepository;
            _buildRepository = buildRepository;
            _divisionMailerRepository = divisionMailerRepository;
            _divisionBrokerRepository = divisionBrokerRepository;
            _buildTableLayoutRepository = buildTableLayoutRepository;

            _mailerRepository = mailerRepository;
            _offerRepository = offerRepository;
            _userGroupRepository = userGroupRepository;
            _groupBrokerRepository = groupBrokerRepository;
            _customCampaignMaxPerRepository = customCampaignMaxPerRepository;
            _appConfiguration = env.GetAppConfiguration();
            _orderExportPartsAppService = orderExportPartsAppService;
            _autoSuppressRepository = autoSuppressRepository;
            _buildtblLayoutRepository = buildtblLayoutRepository;
            _buildTableRepository = buildTableRepository;
            _decoyRepository = decoyRepository;
            _campaignBillingRepository = campaignBillingRepository;
            _lookupCache = lookupCache;
            _userCache = userCache;
            _buildCache = buildCache;
            _utils = utils;
            _shortSearch = shortSearch;
            _redisCacheHelper = redisCacheHelper;
            _permissionChecker = permissionChecker;
            _segmentSelectionsAppService = segmentSelectionsAppService;
            Logger = NullLogger.Instance;
            _subSelectRepository = subSelectRepository;
            _segmentListRepository = segmentListRepository;
        }

        #region Create Campaign
        [AbpAuthorize(AppPermissions.Pages_Campaigns_Create,AppPermissions.Pages_FastCount_Create,AppPermissions.Pages_PlaceOrder)]
        private async Task<GetCampaignsListForView> CreateAsync(CreateOrEditCampaignDto input)
        {
            GetCampaignsListForView campaignDetails;
            CampaignDistinctFields distinctFields;
            var iOfferId = 0;
            var iMailerId = 0;
            var build = 0;
            //Get the latest build only if user has MailerID else get BuildID from UI.
            //var buildID = input.GeneralData.HasUserMailer ? _buildAppService.GetLatestBuildFromDatabaseID(input.GeneralData.DatabaseID) : input.GeneralData.BuildID;
            var buildID = input.GeneralData.BuildID;
            if (!buildID.Equals(0))
            {
                try
                {
                    // Fetch distinct fields based on order                       
                    distinctFields = GetDistinctFields(input.GeneralData);
                    // Keeping Offer & Mailer for future use.
                    iOfferId = distinctFields.OfferID <= 0 ? 0 : distinctFields.OfferID;
                    iMailerId = distinctFields.MailerID;
                    // adding campaign entry in tblOrder
                    campaignDetails = await CreateCampaignAsync(input, distinctFields);
                    campaignDetails.DatabaseName = _databaseRepository.GetAll().FirstOrDefault(x => x.Id == campaignDetails.DatabaseID).cDatabaseName;
                    if (!string.IsNullOrEmpty(campaignDetails.DatabaseName) && (campaignDetails.DatabaseName.ToLower().StartsWith(DatabaseNameConst.Infogroup) || campaignDetails.DatabaseName.ToLower().EndsWith(DatabaseNameConst.Database)))
                    {
                        campaignDetails.DatabaseName = campaignDetails.DatabaseName.Replace(DatabaseNameConst.Database, "", StringComparison.OrdinalIgnoreCase);
                        campaignDetails.DatabaseName = campaignDetails.DatabaseName.Replace(DatabaseNameConst.Infogroup, "", StringComparison.OrdinalIgnoreCase);
                    }

                    if (campaignDetails.CampaignId != 0)
                    {
                        //adding entry in tblOrderStatus
                        await CreateOrderStatusAsync(campaignDetails.CampaignId);

                        //adding entry in tblsegment for order level
                        await CreateOrderLevelSegmentAsync(campaignDetails.CampaignId, buildID, input.GeneralData.DatabaseID, input.GeneralData.cChannelType);

                        //adding entry in tblsegment for default segment
                        campaignDetails.SegmentID = await CreateDefaultSegmentAsync(campaignDetails.CampaignId, buildID, input.GeneralData.DatabaseID);

                        // OrderMaxPers
                        await CreateCampaignMaxPerAsync(input, campaignDetails.CampaignId);

                        //fetching CAS approval details and if available adding it to tblOrderCASApproval
                        await CreateCASApprovalAsync(iOfferId, buildID, campaignDetails.CampaignId);

                        // For Decoy
                        await CreateCampaignDecoyAsync(iMailerId, campaignDetails.CampaignId);

                        var buildDetails = _buildRepository.GetBuildHierarchyDetails(input.GeneralData.BuildID);
                        campaignDetails.DatabaseID = buildDetails.DatabaseId;
                        campaignDetails.DivisionId = buildDetails.DivisionId;
                        var cBuild = buildDetails.cBuild;
                        int.TryParse(cBuild, out build);
                        campaignDetails.Build = build;
                    }
                    return campaignDetails;
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
            else
            {
                if (input.GeneralData.HasUserMailer)
                    throw new UserFriendlyException(L("latestBuildNotAvailable"));
                else
                    throw new UserFriendlyException(L("NoBuildSelected"));
            }
        }
        public async Task<GetCampaignsListForView> CreateOrEditAsync(CreateOrEditCampaignDto input, int orderStatus)
        {
            try
            {
                if (input.Id.HasValue)
                {
                    if (input.listOfXTabRecords != null && input.listOfXTabRecords.Count > 0)
                        SaveReports(input.listOfXTabRecords, Convert.ToInt32(input.Id));

                    if (input.listOfMultidimensionalRecords != null && input.listOfMultidimensionalRecords.Count > 0)
                        SaveMultidimensionalReports(input.listOfMultidimensionalRecords, Convert.ToInt32(input.Id));
                    SaveMaxPerRecords(input.maxPerData);
                    if (input.decoyData != null && input.decoyData.listOfDecoys.Count > 0)
                        SaveDecoys(input.decoyData, Convert.ToInt32(input.Id));
                    return await UpdateAsync(input, orderStatus);
                }
                else
                {
                    var campaignDetails = await CreateAsync(input);
                    _userCache.SetDefaultDatabaseForCampaign(Convert.ToInt32(_mySession.UserId), input.GeneralData.DatabaseID);
                    input.maxPerData.CampaignId = campaignDetails.CampaignId;
                    SaveMaxPerRecords(input.maxPerData, true);
                    return campaignDetails;
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        private async Task<GetCampaignsListForView> CreateCampaignAsync(CreateOrEditCampaignDto input, CampaignDistinctFields distinctFields)
        {
            var campaignDetails = new GetCampaignsListForView
            {
                DatabaseID = input.GeneralData.DatabaseID,
                BuildID = distinctFields.BuildID,
                MailerId = distinctFields.MailerID
            };

            var shipNotes = string.Empty;
            if (input.campaignOutputDto != null && input.campaignOutputDto.ShipNotes != null) {
                shipNotes = input.campaignOutputDto.ShipNotes;
            }

            var newCampaign = new Campaign
            {
                BuildID = distinctFields.BuildID,
                MailerID = distinctFields.MailerID,
                OfferID = distinctFields.OfferID,
                cOrderType = input.GeneralData.cOrderType,
                UserID = _mySession.IDMSUserId,
                iIsOrder = false,
                cDescription = input.GeneralData.cDescription,
                iProvidedCount = 0,
                iIsRandomExecution = false,
                dDateLastRun = DateTime.Now,
                cSortFields = string.Empty,
                cOrderNo = string.Empty,
                cNotes = shipNotes,
                iDecoyQty = 0,
                iDecoyKeyMethod = 1,
                cDecoyKey = string.Empty,
                cDecoysByKeycode = true,
                cSpecialProcess = string.Empty,
                cShiptoType = string.Empty,
                cShipTOEmail = string.Empty,
                cShipCCEmail = string.Empty,
                cShipSUBJECT = string.Empty,
                LK_ExportFileFormatID = "FF",
                iSplitType = 1,
                iSplitIntoNParts = 0,
                dCreatedDate = DateTime.Now,
                cCreatedBy = _mySession.IDMSUserName,
                cOutputCase = "U",
                DivisionMailerID = distinctFields.DivisionMailerID,
                DivisionBrokerID = distinctFields.DivisionBrokerID,
                cOfferName = distinctFields.cOfferName,
                iMinQuantityOrderLevelMaxPer = 0,
                iMaxQuantityOrderLevelMaxPer = 0,
                cMaxPerFieldOrderLevel = string.Empty,
                cChannelType = distinctFields.cChannelType,
                cFileLabel = string.Empty,
                iIsExportDataFileOnly = false,
                iAvailableQty = 0,
                LK_Media = "E",
                cDatabaseName = string.Empty,
                iIsAddHeader = false,
                cExportLayout = string.Empty,
                iHouseFilePriority = false,
                cDecoyKey1 = string.Empty,
                LK_AccountCode = "006000",
                cLVAOrderNo = string.Empty,
                cNextMarkOrderNo = string.Empty,
                cBrokerPONo = string.Empty,
                iIsNoUsage = false,
                iIsNetUse = false,
                iExportLayoutID = 0,
                LK_PGPKeyFile = string.Empty,
                iIsUncompressed = false
            };

            campaignDetails.CampaignId = await _campaignRepository.InsertAndGetIdAsync(newCampaign);
            CurrentUnitOfWork.SaveChanges();
            return campaignDetails;
        }

        private async Task<CampaignDistinctFields> GetDefaultFields(CampaignGeneralDto input, int buildID)
        {
            //Get the Mailer, DivisionMailer and DivisionBroker for the logged in user
            var user = _userRepository.Get(_mySession.IDMSUserId);
            //Get the latest offer based on the databaseID and BuildID
            var query = _campaignBizness.getOfferByMailerQuery(user.MailerID);
            var campaignsListForView = await _customCampaignRepository.getOfferMailerBuild(query.Item1, query.Item2);
            var distinctFields = new CampaignDistinctFields
            {
                BuildID = buildID,
                cChannelType = "P",
                cOfferName = input.cOfferName,
                DivisionMailerID = user.DivisionMailerID,
                DivisionBrokerID = user.DivisionBrokerID,
                MailerID = user.MailerID,
                OfferID = campaignsListForView.OfferID
            };
            return distinctFields;
        }


        private CampaignDistinctFields GetDistinctFields(CampaignGeneralDto input)
        {
            var distinctFields = new CampaignDistinctFields
            {
                BuildID = input.BuildID,
                cChannelType = input.cChannelType,
                DivisionMailerID = 0,
                DivisionBrokerID = 0,
            };
            if (input.DivisionalDatabase)
            {
                var divisionMailerID = input.Mailer?.Value ?? 0;
                var divisionBrokerID = input.Broker?.Value ?? 0;
                distinctFields.MailerID = FetchGenericMailerForDB(input.DatabaseID);
                distinctFields.OfferID = FetchGenericOfferForMailer(distinctFields.MailerID);
                distinctFields.DivisionMailerID = Convert.ToInt32(divisionMailerID);
                distinctFields.DivisionBrokerID = Convert.ToInt32(divisionBrokerID);
                distinctFields.cOfferName = input.cOfferName;
            }
            else
            {
                var mailerId = input.Mailer?.Value ?? 0;
                mailerId = Convert.ToInt32(mailerId);
                if (Convert.ToInt32(mailerId) != 0)
                {
                    distinctFields.MailerID = Convert.ToInt32(mailerId);
                }

                distinctFields.OfferID = input.OfferID;
            }
            distinctFields.cOfferName = string.IsNullOrEmpty(distinctFields.cOfferName) ? string.Empty : distinctFields.cOfferName;

            return distinctFields;
        }

        public async Task<bool> ValidateCampaignForSAN(int campaignId)
        {
            bool bolReturnValue;
            var endpointaddress = _appConfiguration["Services:Uri"];
            var service = new IDMSCommonService.IDMSIQServiceClient(endpointaddress);
            var verifyCountInputs = await service.VerifyCountInputsAsync(campaignId);
            var validationMsg = string.Empty;
            if (!string.IsNullOrEmpty(verifyCountInputs.VerifyCountInputsResult.LogMessages))
            {
                validationMsg = verifyCountInputs.VerifyCountInputsResult.LogMessages.Replace("\r\n", "\n").Replace("'", "");
            }
            if (verifyCountInputs.VerifyCountInputsResult.IsVerifyCountInputs || validationMsg.ToLower().Contains("amazon redshift") || validationMsg.ToLower().Contains("san validation failed for field"))
            {
                bolReturnValue = true;
            }
            else
            {
                throw new UserFriendlyException(validationMsg);
            }
            return bolReturnValue;
        }

        #endregion

        #region CreateOrderStatus
        private async Task CreateOrderStatusAsync(int campaignId)
        {
            OrderStatus orderStatus = new OrderStatus
            {
                OrderID = campaignId,
                iStatus = 10,
                iIsCurrent = true,
                cNotes = string.Empty,
                iStopRequested = false,
                dCreatedDate = DateTime.Now,
                cCreatedBy = _mySession.IDMSUserName
            };
            await _orderStatusRepository.InsertAsync(orderStatus);
        }
        #endregion

        #region CreateCampaignDecoy        
        private async Task CreateCampaignDecoyAsync(int iMailerId, int campaignId)
        {
            var decoyEntity = await _customDecoyRepository.GetDecoyEntityListByMailer(iMailerId);
            if (decoyEntity != null && decoyEntity.Count > 0)
            {
                foreach (DecoyDto decoy in decoyEntity)
                {
                    var cDecoy = new CampaignDecoy
                    {
                        cAddress1 = decoy.cAddress1,
                        cAddress2 = decoy.cAddress2,
                        cDecoyType = decoy.cDecoyType,
                        cCity = decoy.cCity,
                        cCompany = decoy.cCompany,
                        cEmail = decoy.cEmail,
                        cFax = decoy.cFax,
                        cFirstName = decoy.cFirstName,
                        cLastName = decoy.cLastName,
                        cDecoyGroup = "A",
                        cName = decoy.cName,
                        cPhone = decoy.cPhone,
                        cState = decoy.cState,
                        cTitle = decoy.cTitle,
                        cZip = decoy.cZip,
                        cZip4 = decoy.cZip4,
                        cKeyCode1 = decoy.cKeyCode1,
                        OrderId = campaignId,
                        dCreatedDate = DateTime.Now,
                        cCreatedBy = _mySession.IDMSUserName
                    };
                    await _campaignDecoyRepository.InsertAsync(cDecoy);
                }
                CurrentUnitOfWork.SaveChanges();
            }
        }
        #endregion

        #region CreateOrderLevelSegment       
        private async Task CreateOrderLevelSegmentAsync(int campaignId, int buildId, int databaseId, string channelType)
        {
            Segment newOrderLevelSegment = new Segment()
            {
                OrderId = campaignId,
                cDescription = "Order Level",
                iDedupeOrderSpecified = 0,
                cTitleSuppression = string.Empty,
                cCreatedBy = _mySession.IDMSUserName,
                iGroup = 0,
                iIsRandomRadiusNth = false,
                iIsOrderLevel = true,
                //iUseAutosuppress = true,
                cKeyCode1 = string.Empty,
                cKeyCode2 = string.Empty,
                iRequiredQty = 0,
                iProvidedQty = 0,
                iDedupeOrderUsed = 0,
                cMaxPerGroup = "00",
                cFixedTitle = string.Empty,
                iDoubleMultiBuyerCount = 0,
                dCreatedDate = DateTime.Now,
                iOutputQty = -1,
                iAvailableQty = 0,
                iUseAutosuppress = CheckAutoSupress(buildId, databaseId),
                cNthPriorityField = string.Empty,
                cNthPriorityFieldOrder = string.Empty,
            };

            var orderLevelSegmentID = await _segmentRepository.InsertAndGetIdAsync(newOrderLevelSegment);
            await _segmentSelectionsAppService.AddDefaultSelections(orderLevelSegmentID, buildId, databaseId, channelType);

            CurrentUnitOfWork.SaveChanges();
        }

        public bool CheckAutoSupress(int buildId, int databaseId)
        {
            try
            {
                var isAutoSupress = from btl in _buildTableLayoutRepository.GetAll()
                                    join bt in _buildTableRepository.GetAll()
                                    on btl.BuildTableId equals bt.Id
                                    join b in _buildRepository.GetAll()
                                    on bt.BuildId equals b.Id
                                    join aus in _autoSuppressRepository.GetAll()
                                    on btl.cFieldName equals aus.cQuestionFieldName
                                    where b.Id == buildId && aus.iIsActive && aus.DatabaseID == databaseId
                                    select new
                                    {
                                        btl.cFieldDescription,
                                        aus
                                    };
                return isAutoSupress != null ? (isAutoSupress.Count() > 0 ? true : false) : false;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        #endregion

        #region CreateDefaultSegment        
        private async Task<int> CreateDefaultSegmentAsync(int campaignId, int buildId, int databaseId)
        {
            var isRandomRadiusNth = _idmsConfigurationCache.GetConfigurationValue("CalculateDistance", databaseId).cValue == "0" ? true : false;
            var newSegmentLevelSegment = new Segment
            {
                OrderId = campaignId,
                cDescription = L("DefaultSegmentDescription"),
                iDedupeOrderSpecified = 1,
                cTitleSuppression = L("DefaultTiltleSupressionName"),
                cCreatedBy = _mySession.IDMSUserName,
                iGroup = 1,
                iIsRandomRadiusNth = isRandomRadiusNth,
                iIsOrderLevel = false,
                cKeyCode1 = string.Empty,
                cKeyCode2 = string.Empty,
                iRequiredQty = 0,
                iProvidedQty = 0,
                iDedupeOrderUsed = 0,
                cMaxPerGroup = "00",
                cFixedTitle = string.Empty,
                iDoubleMultiBuyerCount = 0,
                dCreatedDate = DateTime.Now,
                iOutputQty = -1,
                iAvailableQty = 0,
                iUseAutosuppress = CheckAutoSupress(buildId, databaseId),
                cNthPriorityField = string.Empty,
                cNthPriorityFieldOrder = string.Empty
            };

            var segmentId = await _segmentRepository.InsertAndGetIdAsync(newSegmentLevelSegment);
            CurrentUnitOfWork.SaveChanges();
            return segmentId;
        }
        #endregion

        #region CreateCampaignMaxPer        
        private async Task CreateCampaignMaxPerAsync(CreateOrEditCampaignDto input, int campaignId)
        {
            var lstLookup = _lookupCache.GetLookUpFields("MAXPER").Where(lookUpItem => lookUpItem.cCode != "00").ToList();
            foreach (LookupCacheItem lk in lstLookup)
            {
                CampaignMaxPer campaignMaxPer = new CampaignMaxPer();
                if (lk.cDescription.Trim().Length > 0)
                {
                    campaignMaxPer.cGroup = lk.cCode;
                    campaignMaxPer.iMaxPerQuantity = 0;
                    campaignMaxPer.cMaxPerField = string.Empty;
                    campaignMaxPer.OrderId = campaignId;
                    campaignMaxPer.dCreatedDate = DateTime.Now;
                    campaignMaxPer.cCreatedBy = _mySession.IDMSUserName;
                }
                await _campaignMaxPerRepository.InsertAsync(campaignMaxPer);
            }
            CurrentUnitOfWork.SaveChanges();
        }
        #endregion

        #region CreateCASApproval
        private async Task CreateCASApprovalAsync(int iOfferId, int buildID, int campaignId)
        {
            var campaignsCASApproval = await _customCASApprovalRepository.getCASApprovalList(iOfferId, buildID);
            if (campaignsCASApproval != null && campaignsCASApproval.Count > 0)
            {
                foreach (CampaignCASApprovalDto item in campaignsCASApproval)
                {
                    CampaignCASApproval cCASApproval = new CampaignCASApproval();
                    cCASApproval.OrderId = campaignId;
                    cCASApproval.MasterLOLID = item.MasterLOLID;
                    cCASApproval.nBasePrice = item.nBasePrice;
                    cCASApproval.cCreatedBy = _mySession.IDMSUserName;
                    cCASApproval.dCreatedDate = DateTime.Now;
                    cCASApproval.cStatus = "A";
                    await _campaignCASApprovalRepository.InsertAsync(cCASApproval);
                }
                CurrentUnitOfWork.SaveChanges();
            }
        }
        #endregion

        #region Update Campaign
        [AbpAuthorize(AppPermissions.Pages_Campaigns_Edit)]
        private async Task<GetCampaignsListForView> UpdateAsync(CreateOrEditCampaignDto input, int orderStatus)
        {
            var getCampaignsListForView = new GetCampaignsListForView { CampaignId = input.Id.Value };
            try
            {
                if (orderStatus > 0)
                {
                    var campaign = _campaignRepository.Get(input.Id.Value);
                    // Update edit campaigns - general tab fields
                    campaign.cDescription = input.GeneralData.cDescription;
                    campaign.cOfferName = input.GeneralData.cOfferName;
                    campaign.cOrderType = input.GeneralData.cOrderType;
                    campaign.cChannelType = input.GeneralData.cChannelType;
                    // Update Output tab fields
                    campaign.cSortFields = string.IsNullOrEmpty(input.campaignOutputDto.Sort) ? string.Empty : input.campaignOutputDto.Sort;
                    campaign.LK_ExportFileFormatID = input.campaignOutputDto.Type == " " ? string.Empty : input.campaignOutputDto.Type;
                    // Update Decoy tab fields

                    campaign.cDecoyKey = input.decoyData.isDecoyKeyMethod == 1 ? campaign.cDecoyKey : input.decoyData.decoyKey.Trim();
                    campaign.cDecoyKey1 = input.decoyData.decoyKey1.Trim();
                    campaign.iDecoyKeyMethod = input.decoyData.isDecoyKeyMethod;
                    campaign.cDecoysByKeycode = input.decoyData.decoyByKeyCode;
                    campaign.cExportLayout = input.campaignOutputDto.LayoutDescription;
                    campaign.iIsAddHeader = input.campaignOutputDto.IsHeaderRow;
                    campaign.iIsExportDataFileOnly = input.campaignOutputDto.IsDataFileOnly;
                    campaign.iIsUncompressed = input.campaignOutputDto.IsUnzipped;
                    campaign.LK_PGPKeyFile = string.IsNullOrEmpty(input.campaignOutputDto.PGPKey) ? string.Empty : input.campaignOutputDto.PGPKey;
                    campaign.cShipTOEmail = input.campaignOutputDto.ShipTo;
                    campaign.cShipSUBJECT = input.campaignOutputDto.ShipSubject;
                    campaign.cShipCCEmail = input.campaignOutputDto.ShipCCEmail;
                    campaign.cNotes = input.campaignOutputDto.ShipNotes;
                    campaign.iExportLayoutID = input.campaignOutputDto.LayoutId;
                    campaign.LK_Media = input.campaignOutputDto.Media;
                    campaign.cFileLabel = input.campaignOutputDto.FileLabel;
                    campaign.cSpecialProcess = input.campaignOutputDto.FileNotes;
                    campaign.iSplitIntoNParts = input.campaignOutputDto.SplitIntoNParts;
                    campaign.iSplitType = input.campaignOutputDto.SplitType;
                    campaign.DivisionMailerID = input.GeneralData.Mailer == null ? 0 : Convert.ToInt32(input.GeneralData.Mailer.Value);
                    campaign.DivisionBrokerID = input.GeneralData.Broker == null ? 0 : Convert.ToInt32(input.GeneralData.Broker.Value);
                    //Update Billing fields
                    if (PermissionChecker.IsGranted(AppPermissions.Pages_Campaigns_Billing))
                    {
                        UpdateBillingData(input.BillingData, campaign);
                    }
                    // Update generic details
                    campaign.cModifiedBy = _mySession.IDMSUserName;
                    campaign.dModifiedDate = DateTime.Now;

                    await _campaignRepository.UpdateAsync(campaign);
                    getCampaignsListForView.CampaignDescription = campaign.cDescription?.Trim();

                    if (input.IsStatusChangeRequired)
                        await _orderStatusManager.UpdateOrderStatus(input.Id.GetValueOrDefault(), orderStatus.Equals(Convert.ToInt32(CampaignStatus.OrderFailed)) ? CampaignStatus.OrderFailed : (orderStatus.Equals(Convert.ToInt32(CampaignStatus.OrderCompleted))) ? CampaignStatus.OrderCompleted : (orderStatus.Equals(Convert.ToInt32(CampaignStatus.OutputCompleted))) ? CampaignStatus.OutputCompleted : CampaignStatus.OrderCreated, _mySession.IDMSUserName);
                    if (input.campaignOutputDto.SplitType == 4)
                    {
                        _orderExportPartsAppService.Delete(input.Id.Value);
                        _orderExportPartsAppService.Insert(input.campaignOutputDto.EditCampaignExportPart, _mySession.IDMSUserName);
                    }

                    UpdateCampaignFTPDetails(input.campaignOutputDto, Convert.ToInt32(input.Id));
                    getCampaignsListForView.Status = _orderStatusRepository.FirstOrDefault(o => o.OrderID.Equals(input.Id) && o.iIsCurrent)?.iStatus ?? 0;
                }
                return getCampaignsListForView;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private void UpdateBillingData(CampaignBillingDto billingInput, Campaign campaign)
        {
            var databaseID = _databaseRepository.GetDataSetDatabaseByOrderID(campaign.Id).Id;
            var sPrefix = _idmsConfigurationCache.GetConfigurationValue("OrderPOPrefix", databaseID).cValue;
            var eCurr = (CampaignStatus)_orderStatusRepository.FirstOrDefault(o => o.OrderID == campaign.Id && o.iIsCurrent).iStatus;
            if (eCurr != CampaignStatus.OrderCreated && eCurr != CampaignStatus.OrderSubmitted && eCurr != CampaignStatus.OrderRunning && eCurr != CampaignStatus.OrderFailed)
            {
                if (!sPrefix.ToUpper().Equals("NONE") && !billingInput.IsNoUsage.Value && !string.IsNullOrEmpty(billingInput.LVAOrderNo))
                {
                    if (!billingInput.LVAOrderNo.StartsWith(sPrefix, StringComparison.OrdinalIgnoreCase))
                        throw new UserFriendlyException(L("LVAValidation", sPrefix));
                }
            }
            campaign.cLVAOrderNo = billingInput.LVAOrderNo;
            campaign.iIsNoUsage = billingInput.IsNoUsage;
            campaign.iIsNetUse = billingInput.IsNetUse;
            campaign.cNextMarkOrderNo = string.IsNullOrEmpty(billingInput.NextMarkOrderNo) ? string.Empty : billingInput.NextMarkOrderNo;
            campaign.cBrokerPONo = string.IsNullOrEmpty(billingInput.BrokerPONo) ? string.Empty : billingInput.BrokerPONo;
            campaign.cSANNumber = string.IsNullOrEmpty(billingInput.SANNumber) ? string.Empty : billingInput.SANNumber;
        }

        #endregion

        #region Get All Campaigns        
        public async Task<PagedResultDto<GetCampaignsListForView>> GetAllCampaignsList(GetCampaignListFilters filters)
        {
            try
            {
                var DatabaseIds = _userCache.GetDatabaseIDs(_mySession.IDMSUserId);
                var divisionalDatabaseIDs = _idmsConfigurationCache.GetConfigurationValue("DivisionalDatabases")?.cValue;
                var shortWhere = _shortSearch.GetWhere(PageID.Campaigns, filters.Filter);
                var query = _campaignBizness.GetAllCampaignsQuery(filters, DatabaseIds, divisionalDatabaseIDs, shortWhere);
                var result = await _customCampaignRepository.GetAllCampaignsList(query.Item1, query.Item2, query.Item3, filters, _mySession.IDMSUserName);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Get All Campaign Queue      
        public async Task<List<CampaignQueueDto>> GetAllCampaignQueue(int userId)
        {
            try
            {
                var campaignQueue = await _customCampaignRepository.GetAllCampaignQueue(userId, _mySession.IDMSUserName);
                campaignQueue.ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.cDatabaseName) && (p.cDatabaseName.ToLower().StartsWith(DatabaseNameConst.Infogroup) || p.cDatabaseName.ToLower().EndsWith(DatabaseNameConst.Database)))
                    {
                        p.cDatabaseName = p.cDatabaseName.Replace(DatabaseNameConst.Database, "", StringComparison.OrdinalIgnoreCase);
                        p.cDatabaseName = p.cDatabaseName.Replace(DatabaseNameConst.Infogroup, "", StringComparison.OrdinalIgnoreCase);
                    }
                });
                return campaignQueue;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        #endregion

        #region Copy Campaign  
        public List<GetCampaignsListForView> CreateCopyCampaign(CampaignCopyDto campaign)
        {
            try
            {
                List<GetCampaignsListForView> copyCampaignID = new List<GetCampaignsListForView>();
                var database = _databaseRepository.GetDataSetDatabaseByOrderID(campaign.CampaignId);
                if (!campaign.BuildId.Equals(0))
                {
                    if (campaign != null)
                    {
                        var copyCampaign = new CampaignDto
                        {
                            ID = campaign.CampaignId,
                            cDescription = campaign.cDescription.Trim(),
                            MailerID = Convert.ToInt32(campaign.Mailer?.Value ?? 0),
                            OfferID = campaign.OfferId,
                            DivisionMailerID = Convert.ToInt32(campaign.DivisionalMailer?.Value ?? 0),
                            DivisionBrokerID = Convert.ToInt32(campaign.DivisionalBroker?.Value ?? 0),
                            cCreatedBy = _mySession.IDMSUserName,
                            cOfferName = campaign.cOfferName,
                            BuildID = campaign.BuildId,
                            UserID = _mySession.IDMSUserId
                        };
                        for (var i = 0; i < campaign.NumberOfCopies; i++)
                        {
                            if (campaign.DivisionalDatabase)
                                _customCampaignRepository.CopyDivCampaign(copyCampaign);
                            else
                                _customCampaignRepository.CopyCampaign(copyCampaign);
                        }
                        copyCampaignID = _customCampaignRepository.GetTopNCampaigns(campaign.cDescription.Replace("'","''"), campaign.Mailer.Value.ToString(), campaign.NumberOfCopies, _mySession.IDMSUserName, _mySession.IDMSUserId, campaign.DatabaseId.ToString());
                    }

                    return copyCampaignID;
                }
                else
                {
                    throw new Exception(L("InvalidBuild"));
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Delete Campaign  
        [AbpAuthorize(AppPermissions.Pages_Campaigns_Delete)]
        public async Task DeleteCampaign(int campaignId)
        {
            try
            {
                await _campaignRepository.DeleteAsync(campaignId);
                _redisCacheHelper.KeyDeleteWithPrefix($"*{IDMSUserCacheConsts.FavoriteCampaigns}");
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Get campaign For Copy
        //[AbpAuthorize(AppPermissions.Pages_FastCount_NewSearch)]
        public GetCampaignForCopyOutputDto GetCampaignForCopy(int campaignId)
        {
            try
            {
                var result = new GetCampaignForCopyOutputDto();
                List<DropdownOutputDto> offerList = null;
                var campaign = _campaignRepository.Get(campaignId);
                var databaseId = _buildRepository.Get(campaign.BuildID)?.DatabaseId ?? 0;
                var buildData = GetBuildsForDatabase(databaseId);
                result.Builds = buildData.BuildDropDown;
                var buildId = buildData.DefaultSelection;
                var userDatabaseMailerData = FetchUserDatabaseMailer(_mySession.IDMSUserId, databaseId);
                result.UserDatabaseMailerRecordCount = userDatabaseMailerData.Count;
                var isDivisionalDatabase = GetDivisionMailerBroker(databaseId);
                result.Offers = new List<DropdownOutputDto> { new DropdownOutputDto { Label = "Select Offer" } };
                if (!isDivisionalDatabase)
                    offerList = GetOffersDDForMailer(campaign.MailerID);
                if (!offerList.IsNullOrEmpty())
                {
                    result.Offers.AddRange(offerList);
                }
                var mailer = campaign.MailerID != 0 ? _mailerRepository.Get(campaign.MailerID) : null;
                var divisionMailer = campaign.DivisionMailerID != 0 ? _divisionMailerRepository.Get(campaign.DivisionMailerID) : null;
                var divisionBroker = campaign.DivisionBrokerID != 0 ? _divisionBrokerRepository.Get(campaign.DivisionBrokerID) : null;
                result.CampaignCopyData = new CampaignCopyDto
                {
                    NumberOfCopies = 1,
                    DivisionalDatabase = isDivisionalDatabase,
                    cDescription = $"{campaign.cDescription} (Copy Of {campaign.Id})",
                    OfferId = campaign.OfferID,
                    cOfferName = campaign.cOfferName,
                    BuildId = buildId,
                    DatabaseId = databaseId,
                    CampaignId = campaignId,
                    Mailer = mailer == null ? null : new DropdownOutputDto { Value = mailer.Id, Label = $"{mailer.cCode.Trim()}{(!string.IsNullOrEmpty(mailer.cCode.Trim()) ? "-" : string.Empty)}{ mailer.cCompany } : {mailer.Id}" },
                    DivisionalMailer = divisionMailer == null ? null : new DropdownOutputDto { Value = divisionMailer.Id, Label = $"{divisionMailer.cCode.Trim()}{(!string.IsNullOrEmpty(divisionMailer.cCode.Trim()) ? "-" : string.Empty)}{ divisionMailer.cCompany } : {divisionMailer.Id}" },
                    DivisionalBroker = divisionBroker == null ? null : new DropdownOutputDto { Value = divisionBroker.Id, Label = $"{divisionBroker.cCode.Trim()}{(!string.IsNullOrEmpty(divisionBroker.cCode.Trim()) ? "-" : string.Empty)}{ divisionBroker.cCompany } : {divisionBroker.Id}" }
                };
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Get Campaign details for edit        
        public CreateOrEditCampaignDto GetCampaignForEdit(int campaignid, int databaseId, int divisionId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var createOrEditCampaignDto = new CreateOrEditCampaignDto();
            var currentStatus = _orderStatusRepository.FirstOrDefault(o => o.OrderID == campaignid && o.iIsCurrent)?.iStatus ?? 0;
            var favouriteCampaigns = _userCache.GetCampaignFavourites(_mySession.IDMSUserId);
            try
            {
                if (campaignid != 0)
                {
                    var campaign = _campaignRepository.Get(campaignid);
                    createOrEditCampaignDto = new CreateOrEditCampaignDto
                    {
                        Id = campaign.Id,
                        GeneralData = FetchGeneralData(campaign, databaseId, divisionId),
                        CurrentStatus = currentStatus,
                        reportsData = FetchXtabData(campaign.Id, databaseId),
                        multiReportsData = FetchMultiReports(campaign.Id, databaseId),
                        getOutputData = FetchOutputDetails(campaign, divisionId, currentStatus),
                        maxPerData = FetchMaxPerData(campaign),
                        BillingData = FetchBillingData(campaign),
                        OESSData = FetchOESSDetails(campaignid, campaign.iProvidedCount),
                        decoyData = FetchDecoys(campaign),
                        isFavouriteCampaign = favouriteCampaigns.Any(favourite => favourite.CampaignId == campaignid),
                        divisionBrokerId = campaign.DivisionBrokerID,
                        divisionMailerId = campaign.DivisionMailerID,
                        IsChannelTypeVisible = currentStatus >= 90 ? false : true,
                        DocumentFileSize = Convert.ToInt32(_idmsConfigurationCache.GetConfigurationValue("MAXEMAILATTACHMENTSIZE", databaseId)?.cValue)
                    };
                }
                if (PermissionChecker.IsGranted(AppPermissions.Pages_CampaignAttachments))
                {
                    createOrEditCampaignDto.documentsData = FetchAttachmentsData(campaignid);
                }
                sw.Stop();
                Logger.Info($"\r\n For campaignId:{campaignid}, Total execution time: {sw.Elapsed.TotalSeconds} \r\n");
                return createOrEditCampaignDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        private CampaignGeneralDto FetchGeneralData(Campaign campaign, int databaseID, int divisionId)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var mailer = campaign.DivisionMailerID != 0 ? _divisionMailerRepository.Get(campaign.DivisionMailerID) : null;
                var broker = campaign.DivisionBrokerID != 0 ? _divisionBrokerRepository.Get(campaign.DivisionBrokerID) : null;
                var campainGeneral = new CampaignGeneralDto
                {
                    cDescription = campaign.cDescription,
                    cOfferName = campaign.cOfferName,
                    OfferID = campaign.OfferID,
                    DatabaseID = databaseID,
                    cOrderType = campaign.cOrderType,
                    cChannelType = campaign.cChannelType,
                    Mailer = mailer == null ? null : new DropdownOutputDto { Value = mailer.Id, Label = $"{mailer.cCode.Trim()}{(!string.IsNullOrEmpty(mailer.cCode.Trim()) ? "-" : string.Empty)}{ mailer.cCompany } : {mailer.Id}" },
                    Broker = broker == null ? null : new DropdownOutputDto { Value = broker.Id, Label = $"{broker.cCode.Trim()}{(!string.IsNullOrEmpty(broker.cCode.Trim()) ? "-" : string.Empty)}{ broker.cCompany } : {broker.Id}" },
                    DivisionalDatabase = GetDivisionMailerBroker(databaseID, divisionId)
                };
                sw.Stop();
                Logger.Info($"\r\n ----- For campaignId:{campaign.Id}, Total time for FetchGeneralData: {sw.Elapsed.TotalSeconds} ----- \r\n");
                return campainGeneral;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private CampaignBillingDto FetchBillingData(Campaign campaign)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var billingData = new CampaignBillingDto
                {
                    LVAOrderNo = campaign.cLVAOrderNo,
                    IsNoUsage = campaign.iIsNoUsage,
                    IsNetUse = campaign.iIsNetUse,
                    NextMarkOrderNo = campaign.cNextMarkOrderNo,
                    BrokerPONo = campaign.cBrokerPONo,
                    SANNumber = campaign.cSANNumber,
                };
                sw.Stop();
                Logger.Info($"\r\n ----- For campaignId:{campaign.Id}, Total time for FetchBillingData: {sw.Elapsed.TotalSeconds} ----- \r\n");
                return billingData;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region New Campaign 2 - Form Options
        public GetAllDatabasesDropdownDto GetAllDatabasesForDropdown(int userid, int currentDatabaseId)
        {
            try
            {
                var defaultSelection = 0;
                if (currentDatabaseId > 0)
                    defaultSelection = currentDatabaseId;
                else
                    defaultSelection = _userCache.GetDefaultDatabaseForCampaign(userid, Convert.ToInt32(_mySession.UserId));


                var dropdownValues = _userCache.GetDropdownOptions(_mySession.IDMSUserId, UserDropdown.Databases);
                return new GetAllDatabasesDropdownDto
                {
                    DefaultDatabase = defaultSelection,
                    Databases = dropdownValues
                };

            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
        private GetAllBuildDropdownOutputDto GetBuildsForDatabase(int iDatabaseID)
        {
            try
            {

                var result = new GetAllBuildDropdownOutputDto();
                result.DefaultSelection = (from build in _buildRepository.GetAll()
                                           join userDatabase in _userDatabaseRepository.GetAll()
                                           on build.DatabaseId equals userDatabase.DatabaseId
                                           where build.iIsReadyToUse
                                           && build.iIsOnDisk
                                           && userDatabase.UserId == _mySession.IDMSUserId
                                           && build.DatabaseId == iDatabaseID
                                           orderby build.Id descending
                                           select build.Id).FirstOrDefault();
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
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
        public List<DropdownOutputDto> GetOffersDDForMailer(int iMailerID)
        {
            try
            {
                var result = (from offer in _offerRepository.GetAll()
                              join mailer in _mailerRepository.GetAll()
                              on offer.MailerId equals mailer.Id
                              where offer.iIsActive && mailer.Id == iMailerID
                              select new DropdownOutputDto
                              {
                                  Value = offer.Id,
                                  Label = $"{offer.cOfferName} : {offer.Id}"
                              }).ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }

        public GetCampaignDropdownsDto GetDatabaseWithLatestBuild(int defaultDatabaseId)
        {
            try
            {
                GetCampaignDropdownsDto result;
                var databases = GetAllDatabasesForDropdown(_mySession.IDMSUserId, defaultDatabaseId);
                if (defaultDatabaseId == 0 && databases.Databases.Count() > 0)
                {
                    result = GetFieldsOnDatabaseChange(Convert.ToInt32(databases.Databases[0].Value));
                }
                else {
                    result = GetFieldsOnDatabaseChange(databases.DefaultDatabase);
                }
                result.Databases = databases;
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public GetCampaignDropdownsDto GetFieldsOnDatabaseChange(int iDatabaseID)
        {
            try
            {

                var result = new GetCampaignDropdownsDto
                {
                    Builds = GetBuildsForDatabase(iDatabaseID),
                    DivisionalDatabase = GetDivisionMailerBroker(iDatabaseID)
                };

                if (!result.DivisionalDatabase)
                {
                    result.DefaultMailer = FetchDefaultMailer(iDatabaseID);
                    result.OfferID = FetchDefaultOffer(iDatabaseID);
                    result.isShowCustomer = true;
                }
                else
                {
                    result.DefaultBroker = GetDefaultDivisionalBroker(iDatabaseID);
                    (result.DefaultMailer, result.isShowCustomer) = FetchDivisionalDefaultMailer(iDatabaseID);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        [AbpAuthorize(AppPermissions.Pages_Campaigns)]
        public List<DropdownOutputDto> GetSearchResultForMailer(GetDropdownInput input)
        {
            try
            {
                return input.DivisionalDatabase ? GetDivisionMailers(input) : GetMailers(input);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        private DropdownOutputDto GetDefaultDivisionalBroker(int databaseId)
        {
            var divisionID = _databaseRepository.FirstOrDefault(a => a.Id == databaseId)?.DivisionId ?? 0;
            var divisionBrokerList = _divisionBrokerRepository.GetAll().Where(x => x.DivisionID == divisionID);
            return (from user in _userRepository.GetAll()
                    join broker in divisionBrokerList
                    on user.DivisionBrokerID equals broker.Id
                    where user.Id.Equals(_mySession.IDMSUserId)
                    select new DropdownOutputDto
                    {
                        Value = broker.Id,
                        Label = $"{broker.cCode.Trim()}{(!string.IsNullOrEmpty(broker.cCode.Trim()) ? "-" : string.Empty)}{ broker.cCompany } : {broker.Id}"
                    }).FirstOrDefault();
        }
        private List<DropdownOutputDto> GetMailers(GetDropdownInput input)
        {
            try
            {
                return _sharedRepository.GetDropdownOptionsForNumericValues(GetMailerQuery(input));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }

        private List<DropdownOutputDto> GetDivisionMailers(GetDropdownInput input)
        {
            try
            {
                return _sharedRepository.GetDropdownOptionsForNumericValues(GetDivisionMailerBrokerQuery(input, CampaignDropdownType.DivisionMailer));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public List<DropdownOutputDto> GetDivisionBrokers(GetDropdownInput input)
        {
            try
            {
                return _sharedRepository.GetDropdownOptionsForNumericValues(GetDivisionMailerBrokerQuery(input, CampaignDropdownType.DivisionBroker));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        private int FetchGenericMailerForDB(int databaseId)
        {
            var genericMailerId = (from mailer in _mailerRepository.GetAll()
                                   join groupBroker in _groupBrokerRepository.GetAll()
                                   on mailer.BrokerID equals groupBroker.BrokerID
                                   join userGroup in _userGroupRepository.GetAll()
                                   on groupBroker.GroupID equals userGroup.GroupID
                                   where mailer.iIsActive
                                   && userGroup.UserId == _mySession.IDMSUserId
                                   && mailer.DatabaseID == databaseId
                                   select mailer).FirstOrDefault()?.Id ?? -1;
            if (genericMailerId == -1)
                throw new UserFriendlyException(L("GenericMailerNotConfigured"));
            return genericMailerId;
        }

        private int FetchGenericOfferForMailer(int iMailer)
        {
            var genericOfferId = (from offer in _offerRepository.GetAll()
                                  join mailer in _mailerRepository.GetAll()
                                  on offer.MailerId equals mailer.Id
                                  where offer.iIsActive && mailer.Id == iMailer
                                  select offer).FirstOrDefault()?.Id ?? -1;
            if (genericOfferId == -1)
                throw new UserFriendlyException(L("GenericOfferNotConfigured"));
            return genericOfferId;
        }
        private DropdownOutputDto FetchDefaultMailer(int databaseId)
        {
            DropdownOutputDto result = null;
            var cConfigMailer = _idmsConfigurationCache.GetConfigurationValue(DefaultMailerConfig, databaseId).cValue;
            int iConfigMailer;
            if (int.TryParse(cConfigMailer, out iConfigMailer) && iConfigMailer > 0)
            {
                var mailer = _mailerRepository.Get(iConfigMailer);
                if (mailer != null)
                {
                    result = new DropdownOutputDto
                    {
                        Value = mailer.Id,
                        Label = $"{mailer.cCompany} : {mailer.Id}"
                    };
                }
            }
            return result;

        }
        private (DropdownOutputDto, bool) FetchDivisionalDefaultMailer(int iDatabaseId)
        {
            DropdownOutputDto result = null;
            var isShowCustomer = false;
            var userDatabaseMailerData = _userCache.GetUserDatabaseMailerList(_mySession.IDMSUserId).Where(x => x.DatabaseId == iDatabaseId).FirstOrDefault();
            if (userDatabaseMailerData != null)
            {
                var mailerDetails = _divisionMailerRepository.Get(userDatabaseMailerData.MailerID);
                result = new DropdownOutputDto
                {
                    Value = mailerDetails.Id,
                    Label = $"{mailerDetails.cCompany} : {mailerDetails.Id}"
                };
            }
            else
            {
                var user = _userRepository.Get(_mySession.IDMSUserId);
                if (user.DivisionMailerID > 0)
                {
                    var mailer = _divisionMailerRepository.Get(user.DivisionMailerID);
                    if (mailer != null)
                    {
                        result = new DropdownOutputDto
                        {
                            Value = mailer.Id,
                            Label = $"{mailer.cCompany} : {mailer.Id}"
                        };
                    }
                }
                isShowCustomer = true;
            }
            return (result, isShowCustomer);
        }

        private int? FetchDefaultOffer(int iDatabaseID)
        {
            var cConfigOffer = _idmsConfigurationCache.GetConfigurationValue(DefaultOfferConfig, iDatabaseID).cValue;
            int iConfigOffer;
            if (int.TryParse(cConfigOffer, out iConfigOffer) && iConfigOffer > 0)
            {
                var offer = _offerRepository.Get(iConfigOffer);
                if (offer != null)
                {
                    return offer.Id;
                }
            }
            return null;
        }
        private bool GetDivisionMailerBroker(int iDatabaseID, int iDivisionId = 0)
        {
            int divisionID;
            if (iDivisionId == 0)
            {
                var campaignDatabase = _databaseRepository.Get(iDatabaseID);
                iDivisionId = campaignDatabase.DivisionId;
            }
            var configCValue = _idmsConfigurationCache.GetConfigurationValue(IsDivisionalDBConfig).cValue;
            if (string.IsNullOrEmpty(configCValue)) return false;
            return configCValue.Split(',')
                   .Select(id => { int.TryParse(id, out divisionID); return divisionID; })
                   .Any(id => id == iDivisionId);
        }
        #endregion

        private UserDatabaseMailerDto FetchUserDatabaseMailer(int userId, int databaseId)
        {
            var count = _userCache.GetUserDatabaseMailerList(userId).Count(x => x.DatabaseId.Equals(databaseId));
            var userDatabaseMailer = _userCache.GetUserDatabaseMailerList(userId).FirstOrDefault(x => x.DatabaseId.Equals(databaseId));
            return new UserDatabaseMailerDto
            {
                Count = count,
                input = count <= 0 ? new GetUserDatabaseMailerForViewDto() : userDatabaseMailer
            };
        }

        public UserDatabaseMailerDto FetchUserDatabaseMailerBasedOnUser(int userId)
        {
            var userDatabaseMailerData = _userCache.GetUserDatabaseMailerList(userId);
            if (userDatabaseMailerData.Count() == 1)
            {
                return new UserDatabaseMailerDto
                {
                    Count = 1,
                    DatabaseId = userDatabaseMailerData.FirstOrDefault().DatabaseId
                };
            }
            else
            {
                return new UserDatabaseMailerDto
                {
                    Count = userDatabaseMailerData.Count(),
                    DatabaseId = _userCache.GetDefaultDatabaseForCampaign(userId, Convert.ToInt32(_mySession.UserId))
                };
            }


        }

        public async Task<GetCampaignsListForView> GetCampaignForView(int Id)
        {
            try
            {
                var divisionalDatabaseIds = _idmsConfigurationCache.GetConfigurationValue("DivisionalDatabases")?.cValue;
                return await _customCampaignRepository.GetCampaignById(_campaignBizness.GetCampaignByIdQuery(Id, divisionalDatabaseIds), _mySession.IDMSUserName);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public CampaignLevelMaxPerDto FetchOrderLevelMaxPer(int campaignId)
        {
            try
            {
                var campaign = _campaignRepository.Get(campaignId);
                return new CampaignLevelMaxPerDto
                {
                    cMaxPerFieldOrderLevel = campaign.cMaxPerFieldOrderLevel,
                    cMaximumQuantity = campaign.iMaxQuantityOrderLevelMaxPer,
                    cMinimumQuantity = campaign.iMinQuantityOrderLevelMaxPer
                };
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<DropdownOutputDto> GetCampaignStatus(int Id)
        {
            try
            {
                return await _customCampaignRepository.GetCampaignStatusById(_campaignBizness.GetCampaignStatusByIdQuery(Id));
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private Tuple<string, List<SqlParameter>> GetDivisionMailerBrokerQuery(GetDropdownInput input, CampaignDropdownType dropdownType)
        {
            string tableName;
            var filterWhere = string.Empty;
            var sqlParams = new List<SqlParameter> { new SqlParameter("@DatabaseId", input.DatabaseID) };
            switch (dropdownType)
            {
                case CampaignDropdownType.DivisionBroker:
                    tableName = "tblDivisionBroker";
                    break;
                case CampaignDropdownType.DivisionMailer:
                    tableName = "tblDivisionMailer";
                    break;
                default:
                    throw new UserFriendlyException(this.L("InvalidDropdownType"));
            }
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                filterWhere = ConditionalWhere;
                sqlParams.Add(new SqlParameter("@Filter", input.Filter));
            }
            var sqlQuery = $@"SELECT  M.Id as iValue,
                         CONCAT(ISNULL(NULLIF(LTRIM(RTRIM(M.cCode)), '') + '-', ''),
                         LTRIM(RTRIM(M.cCompany)),' : ' , M.Id  ) as cLabel,
                         M.cCompany
                         FROM {tableName} M  
                         INNER JOIN tblDatabase DB
                         ON M.DivisionID = DB.DivisionID
                         WHERE (M.iIsActive = 1 ) 
                         AND (DB.Id = @DatabaseId )
                         {filterWhere} 
                         ORDER BY M.cCompany ASC;";
            return new Tuple<string, List<SqlParameter>>(sqlQuery, sqlParams);
        }

        private Tuple<string, List<SqlParameter>> GetMailerQuery(GetDropdownInput input)
        {
            var filterWhere = string.Empty;
            var sqlParams = new List<SqlParameter> {
            new SqlParameter("@DatabaseId", input.DatabaseID),
            new SqlParameter("@UserId", _mySession.IDMSUserId)
        };
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                filterWhere = ConditionalWhere;
                sqlParams.Add(new SqlParameter("@Filter", input.Filter));
            }
            var sqlQuery = $@"SELECT DISTINCT M.Id as iValue,
                          CONCAT(ISNULL(NULLIF(LTRIM(RTRIM(M.cCode)), '') + '-', '') , LTRIM(RTRIM(M.cCompany)), ' : ', M.Id  )
                          AS cLabel,
                          M.cCompany
                          FROM tblMailer M WITH (NOLOCK)
                          INNER JOIN tblGroupBroker GB WITH (NOLOCK) ON M.BrokerID=GB.BrokerID
                          INNER JOIN tblUserGroup UG WITH (NOLOCK) ON UG.GroupID = GB.GroupID
                          WHERE M.iIsActive = 1
                          AND EXISTS (SELECT * FROM tblOffer O WITH (NOLOCK) WHERE O.MailerID = M.ID AND O.iIsActive = 1)
                          AND UG.UserID = @UserId
                          AND M.DatabaseID = @DatabaseId
                          {filterWhere}
                          Order BY M.cCompany ASC ;";
            return new Tuple<string, List<SqlParameter>>(sqlQuery, sqlParams);
        }

        #region  Get FastCount Campaign List 
        [AbpAuthorize(AppPermissions.Pages_FastCount_History)]
        public async Task<PagedResultDto<GetCampaignsListForView>> GetAllFastCountCampaignsList(GetFastCountCampaignListFilters filters)
        {
            try
            {
                var DatabaseIds = _userCache.GetDatabaseIDs(_mySession.IDMSUserId);
                var divisionalDatabaseIDs = _idmsConfigurationCache.GetConfigurationValue("DivisionalDatabases")?.cValue;
                var query = _campaignBizness.GetAllFastCountCampaignsQuery(filters,divisionalDatabaseIDs);
                var result = await _customCampaignRepository.GetAllFastCountCampaignsList(query.Item1, query.Item2, query.Item3, _mySession.IDMSUserName);
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
        #region Update MailerId for Fastcount
        public async Task UpdateFastCountMailerId(int campaignId)
        {
            try
            {
                var campaign = _campaignRepository.Get(campaignId);
                campaign.MailerID= Math.Abs(campaign.MailerID);

                if(campaign.OfferID == 0)
                {
                    campaign.OfferID = FetchGenericOfferForMailer(campaign.MailerID);
                }
                await _campaignRepository.UpdateAsync(campaign);
                CurrentUnitOfWork.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        #endregion

        public int GetGenericMailerForDB(int databaseId)
        {
            try
            {
                return FetchGenericMailerForDB(databaseId);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public Dictionary<string, List<string>> getSegmentsWithSourcesAndSubSelect(int campaignId) {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>() {
                { "SOURCE",  getAllSegementsWithSources(campaignId) },
                { "SUBSELECT",  getAllSegementsWithSubselect(campaignId) }
            };

            return dictionary;
        }
        

        private List<string> getAllSegementsWithSources(int campaignId) {
            return (from segment in _segmentRepository.GetAll()
                    join segmentList in _segmentListRepository.GetAll()
                    on segment.Id equals segmentList.SegmentId
                    where segment.OrderId.Equals(campaignId)
                    select segment.cDescription
            ).ToList();
        }

        private List<string> getAllSegementsWithSubselect(int campaignId)
        {
            return (from segment in _segmentRepository.GetAll()
                    join subselect in _subSelectRepository.GetAll()
                    on segment.Id equals subselect.SegmentId
                    where segment.OrderId.Equals(campaignId)
                    select segment.cDescription
            ).ToList();
        }
    }
}
 