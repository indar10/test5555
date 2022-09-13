using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Infogroup.IDMS.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppPermissions"/> for all permission names.
    /// </summary>
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            //var processQueueDatabases = pages.CreateChildPermission(AppPermissions.Pages_ProcessQueueDatabases, L("ProcessQueueDatabases"));
            //processQueueDatabases.CreateChildPermission(AppPermissions.Pages_ProcessQueueDatabases_Create, L("CreateNewProcessQueueDatabase"));
            //processQueueDatabases.CreateChildPermission(AppPermissions.Pages_ProcessQueueDatabases_Edit, L("EditProcessQueueDatabase"));
            //processQueueDatabases.CreateChildPermission(AppPermissions.Pages_ProcessQueueDatabases_Delete, L("DeleteProcessQueueDatabase"));



            var processQueues = pages.CreateChildPermission(AppPermissions.Pages_ProcessQueues, L("ProcessQueues"));
            processQueues.CreateChildPermission(AppPermissions.Pages_ProcessQueues_Create, L("CreateNewProcessQueue"));
            processQueues.CreateChildPermission(AppPermissions.Pages_ProcessQueues_Edit, L("EditProcessQueue"));
            processQueues.CreateChildPermission(AppPermissions.Pages_ProcessQueues_Delete, L("DeleteProcessQueue"));



            //var buildLols = pages.CreateChildPermission(AppPermissions.Pages_BuildLols, L("BuildLols"));
            //buildLols.CreateChildPermission(AppPermissions.Pages_BuildLols_Create, L("CreateNewBuildLol"));
            //buildLols.CreateChildPermission(AppPermissions.Pages_BuildLols_Edit, L("EditBuildLol"));
            //buildLols.CreateChildPermission(AppPermissions.Pages_BuildLols_Delete, L("DeleteBuildLol"));

            var idmsConfiguration = pages.CreateChildPermission(AppPermissions.Pages_IDMSConfiguration, L("IDMSConfiguration"));
            idmsConfiguration.CreateChildPermission(AppPermissions.Pages_IDMSConfiguration_Create, L("CreateNewConfiguration"));

            //var listMailerRequesteds = pages.CreateChildPermission(AppPermissions.Pages_ListMailerRequesteds, L("ListMailerRequesteds"));
            //listMailerRequesteds.CreateChildPermission(AppPermissions.Pages_ListMailerRequesteds_Create, L("CreateNewListMailerRequested"));
            //listMailerRequesteds.CreateChildPermission(AppPermissions.Pages_ListMailerRequesteds_Edit, L("EditListMailerRequested"));
            //listMailerRequesteds.CreateChildPermission(AppPermissions.Pages_ListMailerRequesteds_Delete, L("DeleteListMailerRequested"));



            //var listMailers = pages.CreateChildPermission(AppPermissions.Pages_ListMailers, L("ListMailers"));
            //listMailers.CreateChildPermission(AppPermissions.Pages_ListMailers_Create, L("CreateNewListMailer"));
            //listMailers.CreateChildPermission(AppPermissions.Pages_ListMailers_Edit, L("EditListMailer"));
            //listMailers.CreateChildPermission(AppPermissions.Pages_ListMailers_Delete, L("DeleteListMailer"));



            var databaseBuild = pages.CreateChildPermission(AppPermissions.Pages_DatabaseBuild, L("DatabaseBuild"));
            databaseBuild.CreateChildPermission(AppPermissions.Pages_DatabaseBuild_ListOfList, L("ListOfList"));
            databaseBuild.CreateChildPermission(AppPermissions.Pages_DatabaseBuild_BuildMaintenance, L("BuildMaintenance"));

            var fastCount = pages.CreateChildPermission(AppPermissions.Pages_FastCount, L("FastCount"));
            var newSearch=  fastCount.CreateChildPermission(AppPermissions.Pages_FastCount_NewSearch, L("NewSearch"));
            fastCount.CreateChildPermission(AppPermissions.Pages_FastCount_History, L("History"));
            newSearch.CreateChildPermission(AppPermissions.Pages_FastCount_Create, L("FastCountcreate"));
            newSearch.CreateChildPermission(AppPermissions.Pages_PlaceOrder, L("PlaceOrder"));

            var matchAppends = pages.CreateChildPermission(AppPermissions.Pages_MatchAppends, L("MatchAppends"));
            matchAppends.CreateChildPermission(AppPermissions.Pages_MatchAppends_Create, L("CreateNewMatchAppend"));
            matchAppends.CreateChildPermission(AppPermissions.Pages_MatchAppends_Edit, L("EditMatchAppend"));

            
            var BatchProcessQueue = pages.CreateChildPermission(AppPermissions.Pages_Batchqueue, L("BatchQueues"));

            var idmsTasks = pages.CreateChildPermission(AppPermissions.Pages_IDMSTasks, L("IDMSTasks"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_SetValidEmailFlag, L("SetValidEmailFlag"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_ResendListtoAddressHygiene, L("ResendListtoAddressHygiene"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_LoadMailerUsage, L("LoadMailerUsage"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_ReportwithCustomColumns, L("ReportwithCustomColumns"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_ExportListConversiondata, L("ExportListConversiondata"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_ApogeeDataAppend, L("ApogeeDataAppend"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_ApogeeExport, L("ApogeeExport"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_SearchPreviousCampaignHistory, L("SearchPreviousCampaignHistory"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_CopyBuild, L("CopyBuild"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_LoadOptoutandHardBounce, L("LoadOptoutandHardBounce"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_ExportEmailHygieneData, L("ExportEmailHygieneData"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_ImportEmailHygieneData, L("ImportEmailHygieneData"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_ActivateLinkTable, L("ActivateLinkTable"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_CloseNotificationJob, L("CloseNotificationJob"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_DeleteBuildTask, L("DeleteBuildTask"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_PenetrationReport, L("PenetrationReport"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_BulkUpdateListAction, L("BulkUpdateListAction"));
            idmsTasks.CreateChildPermission(AppPermissions.Pages_IDMSTasks_ModelPivotReport, L("ModelPivotReport"));

            var models = pages.CreateChildPermission(AppPermissions.Pages_Models, L("Models"));
            models.CreateChildPermission(AppPermissions.Pages_Models_Create, L("CreateNewModel_permission"));
            models.CreateChildPermission(AppPermissions.Pages_Models_Edit, L("EditModel_permission"));
            models.CreateChildPermission(AppPermissions.Pages_Models_Copy, L("CopyModel"));
            models.CreateChildPermission(AppPermissions.Pages_Models_ScoreSample, L("ScoreSample"));
            models.CreateChildPermission(AppPermissions.Pages_Models_ScoreDatabase, L("ScoreDatabase"));
            models.CreateChildPermission(AppPermissions.Pages_Models_Cancel, L("Cancel"));

            var report = pages.CreateChildPermission(AppPermissions.Pages_Report, L("Reports"));
            report.CreateChildPermission(AppPermissions.Pages_Reports_ShippedReport, L("ShippedReport"));
            report.CreateChildPermission(AppPermissions.Pages_Reports_SelectionFieldCountReport, L("SelectionFieldCountReport"));

            var maintenance = pages.CreateChildPermission(AppPermissions.Pages_Maintenance, L("Setup"));

            var securityGroups = maintenance.CreateChildPermission(AppPermissions.Pages_SecurityGroups, L("SecurityGroups"));
            securityGroups.CreateChildPermission(AppPermissions.Pages_SecurityGroups_Create, L("CreateNewSecurityGroup"));
            securityGroups.CreateChildPermission(AppPermissions.Pages_SecurityGroups_Edit, L("EditSecurityGroup"));
            securityGroups.CreateChildPermission(AppPermissions.Pages_SecurityGroups_Print, L("PrintSecurityGroup"));

            var divisionMailers = maintenance.CreateChildPermission(AppPermissions.Pages_DivisionMailers, L("DivisionMailers"));
            divisionMailers.CreateChildPermission(AppPermissions.Pages_DivisionMailers_Create, L("CreateNewDivisionMailer"));
            divisionMailers.CreateChildPermission(AppPermissions.Pages_DivisionMailers_Edit, L("EditDivisionalMailer"));
            divisionMailers.CreateChildPermission(AppPermissions.Pages_DivisionMailers_Print, L("PrintDivisionMailer"));
            var adminExportLayouts = maintenance.CreateChildPermission(AppPermissions.Pages_AdminExportLayouts, L("AdminExportLayouts"));
            adminExportLayouts.CreateChildPermission(AppPermissions.Pages_AdminExportLayouts_Create, L("CreateNewAdminExportLayout"));
            adminExportLayouts.CreateChildPermission(AppPermissions.Pages_AdminExportLayouts_Edit, L("EditAdminExportLayout"));
            adminExportLayouts.CreateChildPermission(AppPermissions.Pages_AdminExportLayouts_Delete, L("DeleteAdminExportLayout"));
            adminExportLayouts.CreateChildPermission(AppPermissions.Pages_AdminExportLayouts_Copy, L("CopyAdminExportLayout"));
            adminExportLayouts.CreateChildPermission(AppPermissions.Pages_AdminExportLayouts_Export, L("ExportAdminExportLayout"));
            var divisionShipTos = maintenance.CreateChildPermission(AppPermissions.Pages_DivisionShipTos, L("DivisionShipTos"));
            divisionShipTos.CreateChildPermission(AppPermissions.Pages_DivisionShipTos_Create, L("CreateNewDivisionShipTo"));
            divisionShipTos.CreateChildPermission(AppPermissions.Pages_DivisionShipTos_Edit, L("EditDivisionShipTo"));
            var databases = maintenance.CreateChildPermission(AppPermissions.Pages_Databases, L("Databases"));
            databases.CreateChildPermission(AppPermissions.Pages_Databases_Create, L("CreateNewDatabase"));
            databases.CreateChildPermission(AppPermissions.Pages_Databases_Edit, L("EditDatabase"));
            databases.CreateChildPermission(AppPermissions.Pages_Databases_Print, L("PrintDatabase"));
            databases.CreateChildPermission(AppPermissions.Pages_SavedSelections_Delete, L("DeleteFavouriteRules"));
            var redisCache = maintenance.CreateChildPermission(AppPermissions.Pages_RedisCache, L("redis"));

            var owners = maintenance.CreateChildPermission(AppPermissions.Pages_Owners, L("Owners"));
            owners.CreateChildPermission(AppPermissions.Pages_Owners_Create, L("CreateNewOwner"));
            owners.CreateChildPermission(AppPermissions.Pages_Owners_Edit, L("EditOwner"));
            owners.CreateChildPermission(AppPermissions.Pages_Owners_Print, L("PrintOwner"));

            var decoys = maintenance.CreateChildPermission(AppPermissions.Pages_Decoys, L("Seeds"));
            decoys.CreateChildPermission(AppPermissions.Pages_Decoys_Create, L("Create"));
            decoys.CreateChildPermission(AppPermissions.Pages_Decoys_Edit, L("Edit"));
            decoys.CreateChildPermission(AppPermissions.Pages_Decoys_Delete, L("Delete"));
            decoys.CreateChildPermission(AppPermissions.Pages_Decoys_Print, L("Print"));

            var managers = maintenance.CreateChildPermission(AppPermissions.Pages_Managers, L("Managers"));
            managers.CreateChildPermission(AppPermissions.Pages_Managers_Create, L("CreateNewManager"));
            managers.CreateChildPermission(AppPermissions.Pages_Managers_Edit, L("EditManager"));
            managers.CreateChildPermission(AppPermissions.Pages_Managers_Print, L("PrintManager"));
            managers.CreateChildPermission(AppPermissions.Pages_Managers_PrintContactAssignments, L("ExportManagerContacts"));

            var contacts = owners.CreateChildPermission(AppPermissions.Pages_OwnerContacts, L("Contacts"));
            contacts.CreateChildPermission(AppPermissions.Pages_OwnerContacts_Create, L("CreateNewContact"));
            contacts.CreateChildPermission(AppPermissions.Pages_OwnerContacts_Edit, L("EditContact"));

            var managerContacts = managers.CreateChildPermission(AppPermissions.Pages_ManagerContacts, L("Contacts"));
            managerContacts.CreateChildPermission(AppPermissions.Pages_ManagerContacts_Create, L("CreateNewContact"));
            managerContacts.CreateChildPermission(AppPermissions.Pages_ManagerContacts_Edit, L("EditContact"));

            var mailers = maintenance.CreateChildPermission(AppPermissions.Pages_Mailers, L("Mailers"));
            mailers.CreateChildPermission(AppPermissions.Pages_Mailers_Create, L("Create"));
            mailers.CreateChildPermission(AppPermissions.Pages_Mailers_Edit, L("Edit"));
            mailers.CreateChildPermission(AppPermissions.Pages_Mailers_Print, L("PrintMailers"));

            var ListAutomates = maintenance.CreateChildPermission(AppPermissions.Pages_ListAutomates, L("ListAutomates"));
            ListAutomates.CreateChildPermission(AppPermissions.Pages_ListAutomates_Create, L("CreateNewIListAutomate"));
            ListAutomates.CreateChildPermission(AppPermissions.Pages_ListAutomates_Edit, L("EditIListAutomate"));
            ListAutomates.CreateChildPermission(AppPermissions.Pages_ListAutomates_Delete, L("DeleteIListAutomate"));

            var brokers = maintenance.CreateChildPermission(AppPermissions.Pages_Brokers, L("Brokers"));
            brokers.CreateChildPermission(AppPermissions.Pages_Brokers_Create, L("CreateNewBroker"));
            brokers.CreateChildPermission(AppPermissions.Pages_Brokers_Edit, L("Edit"));
            brokers.CreateChildPermission(AppPermissions.Pages_Brokers_Print, L("Print"));

            var Lookups = maintenance.CreateChildPermission(AppPermissions.Pages_Lookups, L("Lookups"));
            Lookups.CreateChildPermission(AppPermissions.Pages_Lookups_Create, L("Create"));
            Lookups.CreateChildPermission(AppPermissions.Pages_Lookups_Edit, L("Edit"));

            var externalBuildTableDatabases = maintenance.CreateChildPermission(AppPermissions.Pages_ExternalBuildTableDatabases, L("ExternalBuildTableDatabase"));
            externalBuildTableDatabases.CreateChildPermission(AppPermissions.Pages_ExternalBuildTableDatabases_Write, L("CreateNewExternalBuildTableDatabase"));


            var contactsBroker = brokers.CreateChildPermission(AppPermissions.Pages_ContactsBrokers, L("Contacts"));
            contactsBroker.CreateChildPermission(AppPermissions.Pages_ContactsBrokers_Create, L("CreateNewContact"));
            contactsBroker.CreateChildPermission(AppPermissions.Pages_ContactsBrokers_Edit, L("EditContact"));

            var mailerContacts = mailers.CreateChildPermission(AppPermissions.Pages_MailerContacts, L("Contacts"));
            mailerContacts.CreateChildPermission(AppPermissions.Pages_MailerContacts_Create, L("Create"));
            mailerContacts.CreateChildPermission(AppPermissions.Pages_MailerContacts_Edit, L("Edit"));



            pages.CreateChildPermission(AppPermissions.Pages_DemoUiComponents, L("DemoUiComponents"));

            // IDMS 
            var reports = pages.CreateChildPermission(AppPermissions.Pages_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);
            reports.CreateChildPermission(AppPermissions.Pages_Dashboard_Edit, L("EditReport"), multiTenancySides: MultiTenancySides.Tenant);

            var campaigns = pages.CreateChildPermission(AppPermissions.Pages_Campaigns, L("Campaigns"));
            campaigns.CreateChildPermission(AppPermissions.Pages_Campaigns_Create, L("CreateNewCampaign"));
            var editCampaign = campaigns.CreateChildPermission(AppPermissions.Pages_Campaigns_Edit, L("EditCampaign"));
            campaigns.CreateChildPermission(AppPermissions.Pages_Campaigns_Cancel, L("CancelCampaign_Permission"));
            campaigns.CreateChildPermission(AppPermissions.Pages_Campaigns_Delete, L("DeleteCampaign_Permission"));
            campaigns.CreateChildPermission(AppPermissions.Pages_Campaigns_Print, L("PrintCampaign"));
            editCampaign.CreateChildPermission(AppPermissions.Pages_Campaigns_Output, L("OutputTab"));
            editCampaign.CreateChildPermission(AppPermissions.Pages_Campaigns_OESS, L("OESSTab"));

            campaigns.CreateChildPermission(AppPermissions.Pages_Campaigns_Reship, L("ReshipCampaign"));
            campaigns.CreateChildPermission(AppPermissions.Pages_Campaigns_Ship, L("ShipCampaign"));
            campaigns.CreateChildPermission(AppPermissions.Pages_Campaigns_Reset, L("ResetCampaign"));
            editCampaign.CreateChildPermission(AppPermissions.Pages_Campaigns_Billing, L("CampaignBilling"));
            var seed = editCampaign.CreateChildPermission(AppPermissions.Pages_CampaignSeeds, L("SeedCampaign"));
            seed.CreateChildPermission(AppPermissions.Pages_Seeds_CampaignDelete, L("SeedCampaignLevelDelete"));

            var campaignsQueue = campaigns.CreateChildPermission(AppPermissions.Pages_Queue, L("Queue"));
            campaignsQueue.CreateChildPermission(AppPermissions.Pages_Queue_Stop, L("StopCampaign"));
            var campaignExportLayouts = campaigns.CreateChildPermission(AppPermissions.Pages_CampaignExportLayouts, L("CampaignExportLayouts"));

            var campaignMaxPers = editCampaign.CreateChildPermission(AppPermissions.Pages_CampaignMaxPers, L("CampaignMaxPers"));

            campaigns.CreateChildPermission(AppPermissions.Pages_CampaignMultiColumnReports, L("CampaignMultiColumnReports"));
            campaigns.CreateChildPermission(AppPermissions.Pages_CampaignXTabReports, L("CampaignXTabReports"));

            var segmentPrevOrderses = campaigns.CreateChildPermission(AppPermissions.Pages_SegmentPrevOrderses, L("SegmentPrevOrderses"));
            segmentPrevOrderses.CreateChildPermission(AppPermissions.Pages_SegmentPrevOrderses_Edit, L("EditSegmentPrevOrders"));
            var campaignAttachments = editCampaign.CreateChildPermission(AppPermissions.Pages_CampaignAttachments, L("CampaignAttachments"));
            var segmentLists = campaigns.CreateChildPermission(AppPermissions.Pages_SegmentLists, L("Sources"));
            campaigns.CreateChildPermission(AppPermissions.Pages_SubSelects, L("SubsetsTitle"));
            segmentLists.CreateChildPermission(AppPermissions.Pages_SegmentLists_Edit, L("EditSources"));
            var segments = campaigns.CreateChildPermission(AppPermissions.Pages_Segments, L("Segments"));
            segments.CreateChildPermission(AppPermissions.Pages_Segments_Create, L("CreateNewSegment"));
            segments.CreateChildPermission(AppPermissions.Pages_Segments_Copy, L("CopySegment"));
            segments.CreateChildPermission(AppPermissions.Pages_Segments_Edit, L("EditSegment"));
            segments.CreateChildPermission(AppPermissions.Pages_Segments_Delete, L("DeleteSegment"));
            segments.CreateChildPermission(AppPermissions.Pages_Segments_Move, L("MoveSegment"));
            segments.CreateChildPermission(AppPermissions.Pages_Segments_DataPreview, L("DataPreviewLinkText"));
            segments.CreateChildPermission(AppPermissions.Pages_Segments_BulkSegments, L("BulkSegmentLinkText"));
            segments.CreateChildPermission(AppPermissions.Pages_Segments_ImportSegments, L("ImportSegments"));

            var savedSelections = campaigns.CreateChildPermission(AppPermissions.Pages_SavedSelections, L("SavedSelections"));

            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));
            
            var offers = mailers.CreateChildPermission(AppPermissions.Pages_Offers, L("Offers"));
            offers.CreateChildPermission(AppPermissions.Pages_Offers_Create, L("Create"));
            offers.CreateChildPermission(AppPermissions.Pages_Offers_Edit, L("Edit"));
            offers.CreateChildPermission(AppPermissions.Pages_Mailers_PrintOffers, L("PrintOffers"));

            var offerSamples = offers.CreateChildPermission(AppPermissions.Pages_OfferSamples, L("OfferSamples"));
            offerSamples.CreateChildPermission(AppPermissions.Pages_OfferSamples_Create, L("Create"));
            offerSamples.CreateChildPermission(AppPermissions.Pages_OfferSamples_Edit, L("Edit"));
            offerSamples.CreateChildPermission(AppPermissions.Pages_OfferSamples_Delete, L("Delete"));

            var otherAccess = administration.CreateChildPermission(AppPermissions.Pages_Administration_OtherAccess, L("OtherAccess"));

            var roles = administration.CreateChildPermission(AppPermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Print, L("PrintRole"));

            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Unlock, L("Unlock"));

            var languages = administration.CreateChildPermission(AppPermissions.Pages_Administration_Languages, L("Languages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Create, L("CreatingNewLanguage"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Edit, L("EditingLanguage"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Delete, L("DeletingLanguages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_ChangeTexts, L("ChangingTexts"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_AuditLogs, L("AuditLogs"));

            var organizationUnits = administration.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles, L("ManagingRoles"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_UiCustomization, L("VisualSettings"));

            //TENANT-SPECIFIC PERMISSIONS

            //pages.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_SubscriptionManagement, L("Subscription"), multiTenancySides: MultiTenancySides.Tenant);

            //HOST-SPECIFIC PERMISSIONS

            var editions = pages.CreateChildPermission(AppPermissions.Pages_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_MoveTenantsToAnotherEdition, L("MoveTenantsToAnotherEdition"), multiTenancySides: MultiTenancySides.Host);

            var tenants = pages.CreateChildPermission(AppPermissions.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, IDMSConsts.LocalizationSourceName);
        }
    }
}