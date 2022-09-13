using Microsoft.Extensions.DependencyInjection;
using Infogroup.IDMS.HealthChecks;

namespace Infogroup.IDMS.Web.HealthCheck
{
    public static class AbpZeroHealthCheck
    {
        public static IHealthChecksBuilder AddAbpZeroHealthCheck(this IServiceCollection services)
        {
            var builder = services.AddHealthChecks();
            builder.AddCheck<IDMSDbContextHealthCheck>("Database Connection");
            builder.AddCheck<IDMSDbContextUsersHealthCheck>("Database Connection with user check");
            builder.AddCheck<CacheHealthCheck>("Cache");

            // add your custom health checks here
            // builder.AddCheck<MyCustomHealthCheck>("my health check");

            return builder;
        }
    }
}
