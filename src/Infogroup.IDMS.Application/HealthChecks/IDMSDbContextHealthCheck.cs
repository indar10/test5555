using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Infogroup.IDMS.EntityFrameworkCore;

namespace Infogroup.IDMS.HealthChecks
{
    public class IDMSDbContextHealthCheck : IHealthCheck
    {
        private readonly DatabaseCheckHelper _checkHelper;

        public IDMSDbContextHealthCheck(DatabaseCheckHelper checkHelper)
        {
            _checkHelper = checkHelper;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (_checkHelper.Exist("db"))
            {
                return Task.FromResult(HealthCheckResult.Healthy("IDMSDbContext connected to database."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("IDMSDbContext could not connect to database"));
        }
    }
}
