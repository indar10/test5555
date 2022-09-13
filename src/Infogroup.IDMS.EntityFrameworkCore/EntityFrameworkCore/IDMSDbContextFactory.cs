using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Infogroup.IDMS.Configuration;
using Infogroup.IDMS.Web;

namespace Infogroup.IDMS.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class IDMSDbContextFactory : IDesignTimeDbContextFactory<IDMSDbContext>
    {
        public IDMSDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IDMSDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(), addUserSecrets: true);

            IDMSDbContextConfigurer.Configure(builder, configuration.GetConnectionString(IDMSConsts.ConnectionStringName));

            return new IDMSDbContext(builder.Options);
        }
    }
}