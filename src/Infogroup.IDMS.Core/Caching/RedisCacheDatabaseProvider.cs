using System;
using Abp.Dependency;
using StackExchange.Redis;

namespace Infogroup.IDMS.Caching
{
    /// <summary>
    /// Implements <see cref="IAbpRedisCacheDatabaseProvider"/>.
    /// </summary>
    public class RedisCacheDatabaseProvider : ISingletonDependency, IRedisCacheDatabaseProvider
    {
        private readonly IRedisConfiguration _redisConfig;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpRedisCacheDatabaseProvider"/> class.
        /// </summary>
        public RedisCacheDatabaseProvider(IRedisConfiguration redisConfig)
        {
            _redisConfig = redisConfig;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase(_redisConfig.DatabaseId);
        }
        public IServer GetServer()
        {
            return _connectionMultiplexer.Value.GetServer(_redisConfig.ConnectionString);
        }
        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect($"{ _redisConfig.ConnectionString},allowAdmin=true");
        }
    }
}
