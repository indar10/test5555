using Infogroup.IDMS.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;

namespace Infogroup.IDMS.Caching
{
    public class RedisConfiguration : IRedisConfiguration
    {
        private readonly IConfigurationRoot _appConfiguration;
        public string ConnectionString => $"{_appConfiguration["Redis:Host"]}:{_appConfiguration["Redis:Port"]}";
        public DistributedCacheEntryOptions CacheEntryOptions
        {
            get
            {
                var options = new DistributedCacheEntryOptions();
                double expirationHours;
                double slidingExpirationHours;
                if (double.TryParse(_appConfiguration["Redis:ExpirationInHours"], out expirationHours))
                {
                    options.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(expirationHours);
                }
                if (double.TryParse(_appConfiguration["Redis:SlidingExpirationInHours"], out slidingExpirationHours))
                {
                    options.SlidingExpiration = TimeSpan.FromHours(slidingExpirationHours);
                }
                return options;
            }
        }

        public int DatabaseId
        {
            get
            {
                int databaseId;
                if (!int.TryParse(_appConfiguration["Redis:DatabaseId"], out databaseId))
                {
                    return -1;
                }
                return databaseId;
            }
        }

        public RedisConfiguration(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }
    }
}