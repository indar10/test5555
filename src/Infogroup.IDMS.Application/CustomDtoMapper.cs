using Infogroup.IDMS.ProcessQueueDatabases.Dtos;
using Infogroup.IDMS.ProcessQueueDatabases;
using Infogroup.IDMS.ProcessQueues.Dtos;
using Infogroup.IDMS.ProcessQueues;
using Infogroup.IDMS.BuildLoLs.Dtos;
using Infogroup.IDMS.BuildLoLs;
using Infogroup.IDMS.ListMailerRequesteds.Dtos;
using Infogroup.IDMS.ListMailerRequesteds;
using Infogroup.IDMS.ListMailers.Dtos;
using Infogroup.IDMS.ListMailers;
using Infogroup.IDMS.ListAutomate.Dtos;
using Infogroup.IDMS.ListAutomate;
using Infogroup.IDMS.SecurityGroups.Dtos;
using Infogroup.IDMS.SecurityGroups;
using Infogroup.IDMS.SubSelectSelections.Dtos;
using Infogroup.IDMS.SubSelectSelections;
using Infogroup.IDMS.Occupations.Dtos;
using Infogroup.IDMS.Occupations;
using Infogroup.IDMS.SysSendMails.Dtos;
using Infogroup.IDMS.SysSendMails;
using Infogroup.IDMS.ListLoadStatuses.Dtos;
using Infogroup.IDMS.ListLoadStatuses;
using Infogroup.IDMS.LoadProcessStatuses.Dtos;
using Infogroup.IDMS.LoadProcessStatuses;
using Infogroup.IDMS.MatchAppendStatuses.Dtos;
using Infogroup.IDMS.MatchAppendStatuses;
using Infogroup.IDMS.MatchAppendOutputLayouts.Dtos;
using Infogroup.IDMS.MatchAppendOutputLayouts;
using Infogroup.IDMS.MatchAppendInputLayouts.Dtos;
using Infogroup.IDMS.MatchAppendInputLayouts;
using Infogroup.IDMS.MatchAppendDatabaseUsers.Dtos;
using Infogroup.IDMS.MatchAppendDatabaseUsers;
using Infogroup.IDMS.MatchAppends.Dtos;
using Infogroup.IDMS.MatchAppends;
using Infogroup.IDMS.IDMSTasks.Dtos;
using Infogroup.IDMS.IDMSTasks;
using Infogroup.IDMS.BatchQueues.Dtos;
using Infogroup.IDMS.BatchQueues;
using Infogroup.IDMS.UserDatabaseAccessObjects.Dtos;
using Infogroup.IDMS.UserDatabaseAccessObjects;
using Infogroup.IDMS.UserAccessObjects.Dtos;
using Infogroup.IDMS.UserAccessObjects;
using Infogroup.IDMS.AccessObjects.Dtos;
using Infogroup.IDMS.AccessObjects;
using Infogroup.IDMS.ModelQueues.Dtos;
using Infogroup.IDMS.ModelQueues;
using Infogroup.IDMS.ModelDetails.Dtos;
using Infogroup.IDMS.ModelDetails;
using Infogroup.IDMS.Models.Dtos;
using Infogroup.IDMS.Models;
using Infogroup.IDMS.Neighborhoods.Dtos;
using Infogroup.IDMS.Neighborhoods;
using Infogroup.IDMS.SubSelectLists.Dtos;
using Infogroup.IDMS.SubSelectLists;
using Infogroup.IDMS.SubSelects.Dtos;
using Infogroup.IDMS.SubSelects;
using Infogroup.IDMS.UserDatabaseMailers.Dtos;
using Infogroup.IDMS.UserDatabaseMailers;
using Infogroup.IDMS.SavedSelectionDetails.Dtos;
using Infogroup.IDMS.SavedSelectionDetails;
using Infogroup.IDMS.SavedSelections.Dtos;
using Infogroup.IDMS.SavedSelections;
using Infogroup.IDMS.Reports.Dtos;
using Infogroup.IDMS.Reports;
using Infogroup.IDMS.UserReports.Dtos;
using Infogroup.IDMS.UserReports;
using Infogroup.IDMS.States.Dtos;
using Infogroup.IDMS.States;
using Infogroup.IDMS.Managers.Dtos;
using Infogroup.IDMS.Managers;
using Infogroup.IDMS.OfferSamples.Dtos;
using Infogroup.IDMS.OfferSamples;
using Infogroup.IDMS.IndustryCodes.Dtos;
using Infogroup.IDMS.IndustryCodes;
using Infogroup.IDMS.SICFranchiseCodes.Dtos;
using Infogroup.IDMS.SICFranchiseCodes;
using Infogroup.IDMS.SICCodeRelateds.Dtos;
using Infogroup.IDMS.SICCodeRelateds;
using Infogroup.IDMS.SICCodes.Dtos;
using Infogroup.IDMS.SICCodes;
using Infogroup.IDMS.Mailers.Dtos;
using Infogroup.IDMS.Mailers;
using Infogroup.IDMS.AutoSuppresses.Dtos;
using Infogroup.IDMS.AutoSuppresses;
using Infogroup.IDMS.OrderExportParts.Dtos;
using Infogroup.IDMS.OrderExportParts;
using Infogroup.IDMS.UserSavedSelectionDetails.Dtos;
using Infogroup.IDMS.UserSavedSelectionDetails;
using Infogroup.IDMS.UserSavedSelections.Dtos;
using Infogroup.IDMS.UserSavedSelections;
using Infogroup.IDMS.Contacts.Dtos;
using Infogroup.IDMS.Contacts;
using Infogroup.IDMS.Owners.Dtos;
using Infogroup.IDMS.Owners;
using Infogroup.IDMS.GroupBrokers.Dtos;
using Infogroup.IDMS.GroupBrokers;
using Infogroup.IDMS.DivisionShipTos.Dtos;
using Infogroup.IDMS.DivisionShipTos;
using Infogroup.IDMS.UserGroups.Dtos;
using Infogroup.IDMS.UserGroups;
using Infogroup.IDMS.Offers.Dtos;
using Infogroup.IDMS.Offers;
using Infogroup.IDMS.DivisionBrokers.Dtos;
using Infogroup.IDMS.DivisionBrokers;
using Infogroup.IDMS.UserDatabases.Dtos;
using Infogroup.IDMS.UserDatabases;
using Infogroup.IDMS.ExportLayoutDetails.Dtos;
using Infogroup.IDMS.ExportLayoutDetails;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Infogroup.IDMS.ExportLayouts;
using Infogroup.IDMS.DivisionMailers.Dtos;
using Infogroup.IDMS.DivisionMailers;
using Infogroup.IDMS.CampaignAttachments.Dtos;
using Infogroup.IDMS.CampaignAttachments;
using Infogroup.IDMS.CampaignExportLayouts.Dtos;
using Infogroup.IDMS.CampaignExportLayouts;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.CampaignMultiColumnReports;
using Infogroup.IDMS.SegmentPrevOrderses.Dtos;
using Infogroup.IDMS.SegmentPrevOrderses;
using Infogroup.IDMS.MasterLoLs.Dtos;
using Infogroup.IDMS.MasterLoLs;
using Infogroup.IDMS.CampaignFTPs.Dtos;
using Infogroup.IDMS.CampaignFTPs;
using Infogroup.IDMS.ExternalBuildTableDatabases.Dtos;
using Infogroup.IDMS.ExternalBuildTableDatabases;
using Infogroup.IDMS.CampaignXTabReports.Dtos;
using Infogroup.IDMS.CampaignXTabReports;
using Infogroup.IDMS.CampaignDecoys;
using Infogroup.IDMS.Decoys.Dtos;
using Infogroup.IDMS.Decoys;
using Infogroup.IDMS.CampaignMaxPers.Dtos;
using Infogroup.IDMS.CampaignMaxPers;
using Infogroup.IDMS.CampaignCASApprovals.Dtos;
using Infogroup.IDMS.CampaignCASApprovals;
using Infogroup.IDMS.UserDivisions.Dtos;
using Infogroup.IDMS.UserDivisions;
using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.BuildTables.Dtos;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.Builds.Dtos;
using Infogroup.IDMS.Builds;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using AutoMapper;
using Infogroup.IDMS.Auditing.Dto;
using Infogroup.IDMS.Authorization.Accounts.Dto;
using Infogroup.IDMS.Authorization.Permissions.Dto;
using Infogroup.IDMS.Authorization.Roles;
using Infogroup.IDMS.Authorization.Roles.Dto;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Authorization.Users.Dto;
using Infogroup.IDMS.Authorization.Users.Importing.Dto;
using Infogroup.IDMS.Authorization.Users.Profile.Dto;
using Infogroup.IDMS.CampaignDecoys.Dtos;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.Campaigns.Dtos;
using Infogroup.IDMS.Chat;
using Infogroup.IDMS.Chat.Dto;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Databases.Dtos;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Editions;
using Infogroup.IDMS.Editions.Dto;
using Infogroup.IDMS.Friendships;
using Infogroup.IDMS.Friendships.Cache;
using Infogroup.IDMS.Friendships.Dto;
using Infogroup.IDMS.Localization.Dto;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.Lookups.Dtos;
using Infogroup.IDMS.MultiTenancy;
using Infogroup.IDMS.MultiTenancy.Dto;
using Infogroup.IDMS.MultiTenancy.HostDashboard.Dto;
using Infogroup.IDMS.MultiTenancy.Payments;
using Infogroup.IDMS.MultiTenancy.Payments.Dto;
using Infogroup.IDMS.Notifications.Dto;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.Organizations.Dto;
using Infogroup.IDMS.Segments;
using Infogroup.IDMS.Segments.Dtos;
using Infogroup.IDMS.SegmentSelections;
using Infogroup.IDMS.Brokers.Dtos;
using Infogroup.IDMS.Brokers;
using Infogroup.IDMS.SegmentSelections.Dtos;
using Infogroup.IDMS.Sessions.Dto;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.IDMSUsers.Dtos;
using Infogroup.IDMS.CampaignBillings;
using Infogroup.IDMS.CampaignBillings.Dtos;
using Infogroup.IDMS.IDMSConfigurations.Dtos;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.ListCASContacts;
using Infogroup.IDMS.ListCASContacts.Dtos;


namespace Infogroup.IDMS
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
           configuration.CreateMap<CreateOrEditProcessQueueDatabaseDto, ProcessQueueDatabase>();
           configuration.CreateMap<ProcessQueueDatabase, ProcessQueueDatabaseDto>();
           configuration.CreateMap<CreateOrEditProcessQueueDto, ProcessQueue>();
           configuration.CreateMap<ProcessQueue, ProcessQueueDto>();
           configuration.CreateMap<CreateOrEditBuildLolDto, BuildLol>();
           configuration.CreateMap<BuildLol, BuildLolDto>();
           configuration.CreateMap<CreateOrEditListMailerRequestedDto, ListMailerRequested>();
           configuration.CreateMap<ListMailerRequested, ListMailerRequestedDto>();
           configuration.CreateMap<CreateOrEditListMailerDto, ListMailer>();
           configuration.CreateMap<ListMailer, ListMailerDto>(); 
            configuration.CreateMap<ListCASDto, ListCASContact>();
            configuration.CreateMap<CreateOrEditListCASContacts, ListCASContact>().ReverseMap();
            configuration.CreateMap<CreateOrEditConfigurationDto, IDMSConfiguration>().ReverseMap();
            configuration.CreateMap<CreateOrEditIListAutomateDto, ListAutomates>().ReverseMap();
            configuration.CreateMap<IListAutomateDto, ListAutomates>().ReverseMap();
            configuration.CreateMap<ExcelExportGroupsDto, SecurityGroupDto>().ReverseMap();
            configuration.CreateMap<CreateOrEditSecurityGroupDto, SecurityGroup>().ReverseMap();
            configuration.CreateMap<SecurityGroupDto, SecurityGroup>().ReverseMap();
            configuration.CreateMap<CreateOrEditSubSelectSelectionDto, SubSelectSelection>().ReverseMap();
            configuration.CreateMap<SubSelectSelectionsDTO, SubSelectSelection>().ReverseMap();
            configuration.CreateMap<CreateOrEditOccupationDto, Occupation>().ReverseMap();
            configuration.CreateMap<OccupationDto, Occupation>().ReverseMap();
            configuration.CreateMap<CreateOrEditSysSendMailDto, SysSendMail>().ReverseMap();
            configuration.CreateMap<SysSendMailDto, SysSendMail>().ReverseMap();
            configuration.CreateMap<CreateOrEditListLoadStatusDto, ListLoadStatus>().ReverseMap();
            configuration.CreateMap<ListLoadStatusDto, ListLoadStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditLoadProcessStatusDto, LoadProcessStatus>().ReverseMap();
            configuration.CreateMap<LoadProcessStatusDto, LoadProcessStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditMatchAppendStatusDto, MatchAppendStatus>().ReverseMap();
            configuration.CreateMap<MatchAppendStatusDto, MatchAppendStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditMatchAppendOutputLayoutDto, MatchAppendOutputLayout>().ReverseMap();
            configuration.CreateMap<MatchAppendOutputLayoutDto, MatchAppendOutputLayout>().ReverseMap();
            configuration.CreateMap<CreateOrEditMatchAppendInputLayoutDto, MatchAppendInputLayout>().ReverseMap();
            configuration.CreateMap<MatchAppendInputLayoutDto, MatchAppendInputLayout>().ReverseMap();
            configuration.CreateMap<CreateOrEditMatchAppendDatabaseUserDto, MatchAppendDatabaseUser>().ReverseMap();
            configuration.CreateMap<MatchAppendDatabaseUserDto, MatchAppendDatabaseUser>().ReverseMap();
            configuration.CreateMap<CreateOrEditMatchAppendDto, MatchAppend>().ReverseMap();
            configuration.CreateMap<MatchAppendDto, MatchAppend>().ReverseMap();
            configuration.CreateMap<CreateOrEditIDMSTaskDto, IDMSTasks.IDMSTask>().ReverseMap();
            configuration.CreateMap<IDMSTaskDto, IDMSTasks.IDMSTask>().ReverseMap();
            configuration.CreateMap<CreateOrEditBatchQueueDto, BatchQueue>().ReverseMap();
            configuration.CreateMap<BatchQueueDto, BatchQueue>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserDatabaseAccessObjectDto, UserDatabaseAccessObject>().ReverseMap();
            configuration.CreateMap<UserDatabaseAccessObjectDto, UserDatabaseAccessObject>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserAccessObjectDto, UserAccessObject>().ReverseMap();
            configuration.CreateMap<UserAccessObjectDto, UserAccessObject>().ReverseMap();
            configuration.CreateMap<CreateOrEditAccessObjectDto, AccessObject>().ReverseMap();
            configuration.CreateMap<AccessObjectDto, AccessObject>().ReverseMap();
            configuration.CreateMap<CreateOrEditModelQueueDto, ModelQueue>().ReverseMap();
            configuration.CreateMap<ModelQueueDto, ModelQueue>().ReverseMap();
            configuration.CreateMap<CreateOrEditModelDetailDto, ModelDetail>().ReverseMap();
            configuration.CreateMap<ModelDetailDto, ModelDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditModelDto, Model>().ReverseMap();
            configuration.CreateMap<ModelScoringDto, Model>().ReverseMap();
            configuration.CreateMap<ModelSummaryDto, Model>().ReverseMap();
            configuration.CreateMap<ModelDetailDto, ModelDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditNeighborhoodDto, Neighborhood>().ReverseMap();
            configuration.CreateMap<NeighborhoodDto, Neighborhood>().ReverseMap();
            configuration.CreateMap<MasterLoL, GetSubSelectListForViewDto>();
            configuration.CreateMap<CreateOrEditSubSelectDto, SubSelect>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserDatabaseMailerDto, UserDatabaseMailer>().ReverseMap();
            configuration.CreateMap<UserDatabaseMailerDto, UserDatabaseMailer>().ReverseMap();
            configuration.CreateMap<IDMSConfigurationCacheItem, IDMSConfigurationDto>().ReverseMap();
            configuration.CreateMap<CampaignOESSDto, CampaignBilling>().ReverseMap();
            configuration.CreateMap<UserSavedSelectionDetail, SegmentSelection>().ReverseMap();
            configuration.CreateMap<SavedSelectionDetail, SegmentSelection>().ReverseMap();
            configuration.CreateMap<SavedSelectionDetailDto, SavedSelectionDetail>().ReverseMap();
            configuration.CreateMap<SavedSelectionDto, SavedSelection>().ReverseMap();
            configuration.CreateMap<ReportDto, Report>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserReportDto, UserReport>().ReverseMap();
            configuration.CreateMap<UserReportDto, UserReport>().ReverseMap();
            configuration.CreateMap<CreateOrEditStateDto, State>().ReverseMap();
            configuration.CreateMap<StateDto, State>().ReverseMap();
            configuration.CreateMap<CreateOrEditManagerDto, Manager>().ReverseMap();
            configuration.CreateMap<ManagerDto, Manager>().ReverseMap();
            configuration.CreateMap<CreateOrEditOfferSampleDto, OfferSample>().ReverseMap();
            configuration.CreateMap<OfferSampleDto, OfferSample>().ReverseMap();
            configuration.CreateMap<CreateOrEditIndustryCodeDto, IndustryCode>().ReverseMap();
            configuration.CreateMap<IndustryCodeDto, IndustryCode>().ReverseMap();
            configuration.CreateMap<CreateOrEditSICFranchiseCodeDto, SICFranchiseCode>().ReverseMap();
            configuration.CreateMap<SICFranchiseCodeDto, SICFranchiseCode>().ReverseMap();
            configuration.CreateMap<CreateOrEditSICCodeRelatedDto, SICCodeRelated>().ReverseMap();
            configuration.CreateMap<SICCodeRelatedDto, SICCodeRelated>().ReverseMap();
            configuration.CreateMap<CreateOrEditSICCodeDto, SICCode>().ReverseMap();
            configuration.CreateMap<CreateOrEditMailerDto, Mailer>().ReverseMap();
            configuration.CreateMap<MailerDto, Mailer>().ReverseMap();
            configuration.CreateMap<CreateOrEditAutoSuppressDto, AutoSuppress>().ReverseMap();
            configuration.CreateMap<AutoSuppressDto, AutoSuppress>().ReverseMap();
            configuration.CreateMap<CreateOrEditOrderExportPartDto, OrderExportPart>().ReverseMap();
            configuration.CreateMap<OrderExportPartDto, OrderExportPart>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserSavedSelectionDetailDto, UserSavedSelectionDetail>().ReverseMap();
            configuration.CreateMap<UserSavedSelectionDetailDto, UserSavedSelectionDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserSavedSelectionDto, UserSavedSelection>().ReverseMap();
            configuration.CreateMap<UserSavedSelectionDto, UserSavedSelection>().ReverseMap();
            configuration.CreateMap<CreateOrEditContactDto, Contact>().ReverseMap();
            configuration.CreateMap<ContactDto, Contact>().ReverseMap();
            configuration.CreateMap<CreateOrEditOwnerDto, Owner>()
           //.ForMember(u => u.CreationTime, options => options.MapFrom(input => input.dCreatedDate))
           .ReverseMap();
            configuration.CreateMap<OwnerDto, Owner>().ReverseMap();
            configuration.CreateMap<CreateOrEditDatabaseDto, Database>().ReverseMap();
            configuration.CreateMap<DatabaseDto, Database>().ReverseMap();
            configuration.CreateMap<CreateOrEditGroupBrokerDto, GroupBrokers.GroupBroker>().ReverseMap();
            configuration.CreateMap<GroupBrokerDto, GroupBrokers.GroupBroker>().ReverseMap();
            configuration.CreateMap<CreateOrEditDivisionShipToDto, DivisionShipTo>().ReverseMap();
            configuration.CreateMap<DivisionShipToDto, DivisionShipTo>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserGroupDto, UserGroup>().ReverseMap();
            configuration.CreateMap<UserGroupDto, UserGroup>().ReverseMap();
            configuration.CreateMap<CreateOrEditOfferDto, Offer>().ReverseMap();
            configuration.CreateMap<OfferDto, Offer>().ReverseMap();
            configuration.CreateMap<CreateOrEditDivisionBrokerDto, DivisionBroker>().ReverseMap();
            configuration.CreateMap<DivisionBrokerDto, DivisionBroker>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserDatabaseDto, UserDatabase>().ReverseMap();
            configuration.CreateMap<UserDatabaseDto, UserDatabase>().ReverseMap();
            configuration.CreateMap<CreateOrEditExportLayoutDetailDto, ExportLayoutDetail>().ReverseMap();
            configuration.CreateMap<ExportLayoutDetailDto, ExportLayoutDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditExportLayoutDto, ExportLayout>().ReverseMap();
            configuration.CreateMap<ExportLayoutDto, ExportLayout>().ReverseMap();
            configuration.CreateMap<CreateOrEditDivisionMailerDto, DivisionMailer>().ReverseMap();
            configuration.CreateMap<DivisionMailerDto, DivisionMailer>().ReverseMap();
            configuration.CreateMap<CreateOrEditCampaignAttachmentDto, CampaignAttachment>().ReverseMap();
            configuration.CreateMap<CampaignAttachmentDto, CampaignAttachment>().ReverseMap();
            configuration.CreateMap<CreateOrEditCampaignExportLayoutDto, CampaignExportLayout>().ReverseMap();
            configuration.CreateMap<CampaignExportLayoutDto, CampaignExportLayout>().ReverseMap();
            configuration.CreateMap<CreateOrEditCampaignMultiColumnReportDto, CampaignMultiColumnReport>();
            configuration.CreateMap<CampaignMultiColumnReport, CampaignMultiColumnReportDto>();
            configuration.CreateMap<CreateOrEditSegmentPrevOrdersDto, SegmentPrevOrders>();
            configuration.CreateMap<SegmentPrevOrders, SegmentPrevOrdersDto>();
            configuration.CreateMap<CreateOrEditMasterLoLDto, MasterLoL>();
            configuration.CreateMap<MasterLoL, MasterLoLDto>();
            configuration.CreateMap<CreateOrEditCampaignFTPDto, CampaignFTP>();
            configuration.CreateMap<CampaignFTP, CampaignFTPDto>();
            configuration.CreateMap<CreateOrEditExternalBuildTableDatabaseDto, ExternalBuildTableDatabase>();
            configuration.CreateMap<ExternalBuildTableDatabase, ExternalBuildTableDatabaseDto>();
            configuration.CreateMap<CreateOrEditCampaignXTabReportDto, CampaignXTabReport>();
            configuration.CreateMap<CampaignDecoy, CampaignDecoyDto>();
            configuration.CreateMap<CreateOrEditDecoyDto, Decoy>().ReverseMap();
            configuration.CreateMap<Decoy, DecoyDto>().ReverseMap();
            configuration.CreateMap<DecoyExcelExporterDto, MailerDto>().ReverseMap();
            configuration.CreateMap<CampaignMaxPer, SegmentLevelMaxPerDto>();
            configuration.CreateMap<CreateOrEditCampaignCASApprovalDto, CampaignCASApproval>();
            configuration.CreateMap<CampaignCASApproval, CampaignCASApprovalDto>();
            configuration.CreateMap<CreateOrEditUserDivisionDto, UserDivision>();
            configuration.CreateMap<UserDivision, UserDivisionDto>();
            configuration.CreateMap<CreateOrEditIDMSUserDto, IDMSUser>();
            configuration.CreateMap<IDMSUser, IDMSUserDto>();
            configuration.CreateMap<CreateOrEditBuildTableLayoutDto, BuildTableLayout>();
            configuration.CreateMap<BuildTableLayout, BuildTableLayoutDto>();
            configuration.CreateMap<CreateOrEditBuildTableDto, BuildTable>();
            configuration.CreateMap<BuildTable, BuildTableDto>();
            configuration.CreateMap<CreateOrEditBuildDto, Build>();
            configuration.CreateMap<Build, BuildDto>();
            // IDMS
            configuration.CreateMap<CreateOrEditSegmentDto, Segment>();
            configuration.CreateMap<Segment, SegmentDto>();
            configuration.CreateMap<Segment, SegmentsGlobalChangesDto>();
            configuration.CreateMap<SegmentSelectionDto, SegmentSelection>().ReverseMap();
            configuration.CreateMap<CreateOrEditLookupDto, Lookup>();
            configuration.CreateMap<Lookup, LookupDto>();
            configuration.CreateMap<CreateOrEditDatabaseDto, Database>();
            configuration.CreateMap<Database, DatabaseDto>();
            configuration.CreateMap<CreateOrEditCampaignDto, Campaign>();
            configuration.CreateMap<OrderStatusDto, OrderStatus>();
            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            //Chat
            configuration.CreateMap<ChatMessage, ChatMessageDto>();
            configuration.CreateMap<ChatMessage, ChatMessageExportDto>();

            //Feature
            configuration.CreateMap<FlatFeatureSelectDto, Feature>().ReverseMap();
            configuration.CreateMap<Feature, FlatFeatureDto>();

            //Role
            configuration.CreateMap<RoleEditDto, Role>().ReverseMap();
            configuration.CreateMap<Role, RoleListDto>();
            configuration.CreateMap<UserRole, UserListRoleDto>();

            //Edition
            configuration.CreateMap<EditionEditDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<EditionCreateDto, SubscribableEdition>();
            configuration.CreateMap<EditionSelectDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<Edition, EditionInfoDto>().Include<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<SubscribableEdition, EditionListDto>();
            configuration.CreateMap<Edition, EditionEditDto>();
            configuration.CreateMap<Edition, SubscribableEdition>();
            configuration.CreateMap<Edition, EditionSelectDto>();

            //Payment
            configuration.CreateMap<SubscriptionPaymentDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPaymentListDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPayment, SubscriptionPaymentInfoDto>();

            //Permission
            configuration.CreateMap<Permission, FlatPermissionDto>();
            configuration.CreateMap<Permission, FlatPermissionWithLevelDto>();

            //Language
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageListDto>();
            configuration.CreateMap<NotificationDefinition, NotificationSubscriptionWithDisplayNameDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>()
                .ForMember(ldto => ldto.IsEnabled, options => options.MapFrom(l => !l.IsDisabled));

            //Tenant
            configuration.CreateMap<Tenant, RecentTenant>();
            configuration.CreateMap<Tenant, TenantLoginInfoDto>();
            configuration.CreateMap<Tenant, TenantListDto>();
            configuration.CreateMap<TenantEditDto, Tenant>().ReverseMap();
            configuration.CreateMap<CurrentTenantInfoDto, Tenant>().ReverseMap();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());

            configuration.CreateMap<OwnerDto, ExcelExporterDto>()
               .ForMember(dto => dto.ContactsList, options => options.Ignore());
            configuration.CreateMap<ManagerDto, ExcelExporterDto>()
               .ForMember(dto => dto.ContactsList, options => options.Ignore());
            configuration.CreateMap<BrokersDto, ExcelExporterDto>()
               .ForMember(dto => dto.ContactsList, options => options.Ignore());

            configuration.CreateMap<GetDivisionMailerForViewDto, DivisionMailerExportDto>();

            configuration.CreateMap<MailerDto, ExcelExporterDto>()
             .ForMember(dto => dto.ContactsList, options => options.Ignore());

            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
            configuration.CreateMap<User, ChatUserDto>();
            configuration.CreateMap<User, OrganizationUnitUserListDto>();
            configuration.CreateMap<Role, OrganizationUnitRoleListDto>();
            configuration.CreateMap<CurrentUserProfileEditDto, User>().ReverseMap();
            configuration.CreateMap<UserLoginAttemptDto, UserLoginAttempt>().ReverseMap();
            configuration.CreateMap<ImportUserDto, User>();

            //AuditLog
            configuration.CreateMap<AuditLog, AuditLogListDto>();
            configuration.CreateMap<EntityChange, EntityChangeListDto>();
            configuration.CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

            //Friendship
            configuration.CreateMap<Friendship, FriendDto>();
            configuration.CreateMap<FriendCacheItem, FriendDto>();

            //OrganizationUnit
            configuration.CreateMap<OrganizationUnit, OrganizationUnitDto>();

            //Broker
            configuration.CreateMap<CreateOrEditBrokerDto, Broker>().ReverseMap();
            configuration.CreateMap<BrokersDto, Broker>().ReverseMap();

            //UserDatabaseMailer
            configuration.CreateMap<GetUserDatabaseMailerForViewDto, UserDatabaseMailer>().ReverseMap();
            /* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */
            configuration.CreateMap<MatchAppendDto, MatchAppend>().ReverseMap();
            configuration.CreateMap<MatchAndAppendInputLayoutDto, MatchAppendInputLayout>().ReverseMap();
            configuration.CreateMap<MatchAndAppendOutputLayoutDto, MatchAppendOutputLayout>().ReverseMap();
        }
    }
}