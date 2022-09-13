using Infogroup.IDMS.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Infogroup.IDMS.Test.Base.Web
{
    public class FakeRedisConfiguration : IRedisConfiguration
    {
        public string ConnectionString => "localhost:6379";
        public DistributedCacheEntryOptions CacheEntryOptions
        {
            get
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = System.DateTimeOffset.UtcNow.AddHours(6),
                    SlidingExpiration = System.TimeSpan.FromHours(3)
                };
                return options;
            }
        }

        public int DatabaseId
        {
            get
            {
                return -1;
            }
        }

    }
}
