using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.Identity;

namespace Infogroup.IDMS.Test.Base.DependencyInjection
{
    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            RegisterIdentity(iocManager);

            var builder = new DbContextOptionsBuilder<IDMSDbContext>();
            //var inMemorySqlite = new System.Data.SqlClient.SqlConnection("Server = 172.27.24.76; Database = DW_Admin_Staging; Trusted_Connection = False; User ID = sa; Password = cybage@123;");
            var inMemorySqlite = new System.Data.SqlClient.SqlConnection("Server=10.50.104.12; Database=DW_Admin; Trusted_Connection=False;User ID=WebUser;Password=Infogroup@001;");
            builder.UseSqlServer(inMemorySqlite);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<IDMSDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            inMemorySqlite.Open();
            using (var iDMSDbContext = new IDMSDbContext(builder.Options))
            {
                iDMSDbContext.Database.EnsureCreated();
            }
        }

        private static void RegisterIdentity(IIocManager iocManager)
        {
            var services = new ServiceCollection();

            IdentityRegistrar.Register(services);

            WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);
        }
    }
}
