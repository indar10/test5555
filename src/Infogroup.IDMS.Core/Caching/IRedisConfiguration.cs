using Abp.Dependency;
using Microsoft.Extensions.Caching.Distributed;

namespace Infogroup.IDMS.Caching
{
    public interface IRedisConfiguration: ITransientDependency
    {
        string ConnectionString { get; }
        int DatabaseId { get; }
        DistributedCacheEntryOptions CacheEntryOptions { get; }
    }
}