using Infogroup.IDMS.ProcessQueueDatabases;
using Infogroup.IDMS.ProcessQueues;
using Infogroup.IDMS.BuildLoLs;
using Infogroup.IDMS.ListMailerRequesteds;
using Infogroup.IDMS.ListMailers;
using Infogroup.IDMS.ListAutomate;
using Infogroup.IDMS.AbpUserPasswords;
using Infogroup.IDMS.SecurityGroups;
using Infogroup.IDMS.IntentTopics;
using Infogroup.IDMS.SubSelectSelections;
using Infogroup.IDMS.Occupations;
using Infogroup.IDMS.SysSendMails;
using Infogroup.IDMS.ListLoadStatuses;
using Infogroup.IDMS.LoadProcessStatuses;
using Infogroup.IDMS.MatchAppendStatuses;
using Infogroup.IDMS.MatchAppendOutputLayouts;
using Infogroup.IDMS.MatchAppendInputLayouts;
using Infogroup.IDMS.MatchAppendDatabaseUsers;
using Infogroup.IDMS.MatchAppends;
using Infogroup.IDMS.IDMSTasks;
using Infogroup.IDMS.BatchQueues;
using Infogroup.IDMS.UserDatabaseAccessObjects;
using Infogroup.IDMS.UserAccessObjects;
using Infogroup.IDMS.AccessObjects;
using Infogroup.IDMS.ModelQueues;
using Infogroup.IDMS.ModelDetails;
using Infogroup.IDMS.Models;
using Infogroup.IDMS.Neighborhoods;
using Infogroup.IDMS.SubSelectLists;
using Infogroup.IDMS.SubSelects;
using Infogroup.IDMS.UserDatabaseMailers;
using Infogroup.IDMS.CampaignFavourites;
using Infogroup.IDMS.SavedSelectionDetails;
using Infogroup.IDMS.SavedSelections;
using Infogroup.IDMS.ListCASContacts;
using Infogroup.IDMS.Reports;
using Infogroup.IDMS.UserReports;
using Infogroup.IDMS.States;
using Infogroup.IDMS.Managers;
using Infogroup.IDMS.OfferSamples;
using Infogroup.IDMS.IndustryCodes;
using Infogroup.IDMS.SICFranchiseCodes;
using Infogroup.IDMS.SICCodeRelateds;
using Infogroup.IDMS.SICCodes;
using Infogroup.IDMS.AutoSuppresses;
using Infogroup.IDMS.OrderExportParts;
using Infogroup.IDMS.UserSavedSelectionDetails;
using Infogroup.IDMS.UserSavedSelections;
using Infogroup.IDMS.Contacts;
using Infogroup.IDMS.Owners;
using Infogroup.IDMS.DivisionShipTos;
using Infogroup.IDMS.GroupBrokers;
using Infogroup.IDMS.UserGroups;
using Infogroup.IDMS.Offers;
using Infogroup.IDMS.DivisionBrokers;
using Infogroup.IDMS.UserDatabases;
using Infogroup.IDMS.ExportLayoutDetails;
using Infogroup.IDMS.ExportLayouts;
using Infogroup.IDMS.DivisionMailers;
using Infogroup.IDMS.CampaignAttachments;
using Infogroup.IDMS.CampaignExportLayouts;
using Infogroup.IDMS.CampaignMultiColumnReports;
using Infogroup.IDMS.SegmentPrevOrderses;
using Infogroup.IDMS.SegmentLists;
using Infogroup.IDMS.MasterLoLs;
using Infogroup.IDMS.CampaignFTPs;
using Infogroup.IDMS.ExternalBuildTableDatabases;
using Infogroup.IDMS.CampaignXTabReports;
using Infogroup.IDMS.CampaignDecoys;
using Infogroup.IDMS.Decoys;
using Infogroup.IDMS.CampaignMaxPers;
using Infogroup.IDMS.CampaignCASApprovals;
using Infogroup.IDMS.UserDivisions;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.Builds;
using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Authorization.Roles;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Chat;
using Infogroup.IDMS.Editions;
using Infogroup.IDMS.Friendships;
using Infogroup.IDMS.MultiTenancy;
using Infogroup.IDMS.MultiTenancy.Accounting;
using Infogroup.IDMS.MultiTenancy.Payments;
using Infogroup.IDMS.Storage;
using Infogroup.IDMS.Lookups;
using Infogroup.IDMS.Segments;
using Infogroup.IDMS.Divisions;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Campaigns;
using Infogroup.IDMS.OrderStatuss;
using Infogroup.IDMS.SegmentSelections;
using Infogroup.IDMS.Mailers;
using Infogroup.IDMS.Brokers;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.CampaignBillings;

namespace Infogroup.IDMS.EntityFrameworkCore
{
    public class IDMSDbContext : AbpZeroDbContext<Tenant, Role, User, IDMSDbContext>, IAbpPersistedGrantDbContext
    {
        public virtual DbSet<ProcessQueueDatabase> ProcessQueueDatabases { get; set; }

        public virtual DbSet<ProcessQueue> ProcessQueues { get; set; }

        public virtual DbSet<BuildLol> BuildLols { get; set; }

        public virtual DbSet<ListMailerRequested> ListMailerRequesteds { get; set; }

        public virtual DbSet<ListMailer> ListMailers { get; set; }

        public virtual DbSet<ListAutomate.ListAutomates> IListAutomates { get; set; }

        public virtual DbSet<AbpUserPassword> AbpUserPasswords { get; set; }

        public virtual DbSet<SecurityGroup> SecurityGroups { get; set; }

        public virtual DbSet<IntentTopic> IntentTopics { get; set; }

        public virtual DbSet<SubSelectSelection> SubSelectSelections { get; set; }

        public virtual DbSet<Occupation> Occupations { get; set; }

        public virtual DbSet<SysSendMail> SysSendMails { get; set; }

        public virtual DbSet<ListLoadStatus> ListLoadStatuses { get; set; }

        public virtual DbSet<LoadProcessStatus> LoadProcessStatuses { get; set; }

        public virtual DbSet<MatchAppendStatus> MatchAppendStatuses { get; set; }

        public virtual DbSet<MatchAppendOutputLayout> MatchAppendOutputLayouts { get; set; }

        public virtual DbSet<MatchAppendInputLayout> MatchAppendInputLayouts { get; set; }

        public virtual DbSet<MatchAppendDatabaseUser> MatchAppendDatabaseUsers { get; set; }

        public virtual DbSet<MatchAppend> MatchAppends { get; set; }

        public virtual DbSet<IDMSTasks.IDMSTask> IDMSTasks { get; set; }

        public virtual DbSet<BatchQueue> BatchQueues { get; set; }

        public virtual DbSet<UserDatabaseAccessObject> UserDatabaseAccessObjects { get; set; }

        public virtual DbSet<UserAccessObject> UserAccessObjects { get; set; }

        public virtual DbSet<AccessObject> AccessObjects { get; set; }

        public virtual DbSet<ModelQueue> ModelQueues { get; set; }

        public virtual DbSet<ModelDetail> ModelDetails { get; set; }

        public virtual DbSet<Model> Models { get; set; }

        public virtual DbSet<Neighborhood> Neighborhoods { get; set; }

        public virtual DbSet<SubSelectList> SubSelectLists { get; set; }

        public virtual DbSet<SubSelect> SubSelects { get; set; }

        public virtual DbSet<UserDatabaseMailer> UserDatabaseMailers { get; set; }

        public virtual DbSet<CampaignFavourite> CampaignFavourites { get; set; }

        public virtual DbSet<SavedSelectionDetail> SavedSelectionDetails { get; set; }

        public virtual DbSet<SavedSelection> SavedSelections { get; set; }

        public virtual DbSet<ListCASContact> ListCASContacts { get; set; }

        public virtual DbSet<Report> Reports { get; set; }

        public virtual DbSet<UserReport> UserReports { get; set; }

        public virtual DbSet<State> States { get; set; }

        public virtual DbSet<Manager> Managers { get; set; }

        public virtual DbSet<OfferSample> OfferSamples { get; set; }

        public virtual DbSet<IndustryCode> IndustryCodes { get; set; }

        public virtual DbSet<SICFranchiseCode> SICFranchiseCodes { get; set; }

        public virtual DbSet<SICCodeRelated> SICCodeRelateds { get; set; }

        public virtual DbSet<SICCode> SICCodes { get; set; }

        public virtual DbSet<AutoSuppress> AutoSuppresses { get; set; }

        public virtual DbSet<OrderExportPart> OrderExportParts { get; set; }

        public virtual DbSet<UserSavedSelectionDetail> UserSavedSelectionDetails { get; set; }

        public virtual DbSet<UserSavedSelection> UserSavedSelections { get; set; }

        public virtual DbSet<Contact> Contacts { get; set; }

        public virtual DbSet<Broker> Brokers { get; set; }

        public virtual DbSet<Owner> Owners { get; set; }

        public virtual DbSet<DivisionShipTo> DivisionShipTos { get; set; }

        public virtual DbSet<GroupBrokers.GroupBroker> GroupBrokers { get; set; }

        public virtual DbSet<UserGroup> UserGroups { get; set; }

        public virtual DbSet<Offer> Offers { get; set; }

        public virtual DbSet<Mailer> Mailers { get; set; }
        public virtual DbSet<DivisionBroker> DivisionBrokers { get; set; }

        public virtual DbSet<UserDatabase> UserDatabases { get; set; }

        public virtual DbSet<ExportLayoutDetail> ExportLayoutDetails { get; set; }

        public virtual DbSet<ExportLayout> ExportLayouts { get; set; }

        public virtual DbSet<DivisionMailer> DivisionMailers { get; set; }

        public virtual DbSet<CampaignAttachment> CampaignAttachments { get; set; }

        public virtual DbSet<CampaignExportLayout> CampaignExportLayouts { get; set; }

        public virtual DbSet<CampaignMultiColumnReport> CampaignMultiColumnReports { get; set; }

        public virtual DbSet<SegmentPrevOrders> SegmentPrevOrderses { get; set; }

        public virtual DbSet<SegmentList> SegmentLists { get; set; }

        public virtual DbSet<MasterLoL> MasterLoLs { get; set; }

        public virtual DbSet<CampaignFTP> CampaignFTPs { get; set; }

        public virtual DbSet<ExternalBuildTableDatabase> ExternalBuildTableDatabases { get; set; }

        public virtual DbSet<CampaignXTabReport> CampaignXTabReports { get; set; }

        public virtual DbSet<CampaignDecoy> CampaignDecoys { get; set; }

        public virtual DbSet<Decoy> Decoys { get; set; }

        public virtual DbSet<CampaignMaxPer> CampaignOrderMaxPers { get; set; }

        public virtual DbSet<CampaignCASApproval> CampaignCASApprovals { get; set; }

        public virtual DbSet<UserDivision> UserDivisions { get; set; }

        public virtual DbSet<IDMSUser> IDMSUsers { get; set; }

        public virtual DbSet<IDMSConfiguration> IDMSConfigurations { get; set; }

        public virtual DbSet<BuildTableLayout> BuildTableLayouts { get; set; }

        public virtual DbSet<BuildTable> BuildTables { get; set; }

        public virtual DbSet<Build> Builds { get; set; }

        /* Define an IDbSet for each entity of the application */
        // IDMS
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<SegmentSelection> SegmentSelections { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }

        public virtual DbSet<Database> Databases { get; set; }

        public virtual DbSet<Campaign> Campaigns { get; set; }

        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

        public virtual DbSet<Segment> Segments { get; set; }

        // END
        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual DbSet<CampaignBilling> CampaignBilling { get; set; }

        public IDMSDbContext(DbContextOptions<IDMSDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BinaryObject>(b =>
                       {
                           b.HasIndex(e => new { e.TenantId });
                       });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
            });

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}