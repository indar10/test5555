using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.EntityFrameworkCore
{
    public static class IDMSDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<IDMSDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<IDMSDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}