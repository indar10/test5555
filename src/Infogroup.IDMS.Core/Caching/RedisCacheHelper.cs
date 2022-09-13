using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Infogroup.IDMS.Caching
{
    public class RedisCacheHelper : IRedisCacheHelper
    {
        private readonly IDistributedCache _cacheManager;
        private readonly IRedisConfiguration _redisConfig;
        private readonly IRedisCacheDatabaseProvider _redisCacheDatabaseProvider;
        private IDatabase _database;

        public RedisCacheHelper(
            IRedisConfiguration redisConfig,
            IRedisCacheDatabaseProvider redisCacheDatabaseProvider,
            IDistributedCache cacheManager)
        {
            _cacheManager = cacheManager;
            _redisConfig = redisConfig;
            _redisCacheDatabaseProvider = redisCacheDatabaseProvider;
        }
        public void SetString(string key, string value , bool setExpiration)
        {
            if (!string.IsNullOrEmpty(key))
            {
                key = key.ToUpper();
                if (setExpiration) _cacheManager.SetString(key, value, _redisConfig.CacheEntryOptions);
                else _cacheManager.SetString(key, value);
            }               
        }
        public string GetString(string key) {
            if (!string.IsNullOrEmpty(key)) key = key.ToUpper();
            return _cacheManager.GetString(key);
        }

        public int KeyCountByPrefix(string prefix)
        {
            _database = _redisCacheDatabaseProvider.GetDatabase();
            prefix = $"{prefix.ToUpper()}*";
            var retVal = _database.ScriptEvaluate("return table.getn(redis.call('keys', ARGV[1]))", values: new RedisValue[] { prefix });

            if (retVal.IsNull)
            {
                return 0;
            }
            return (int)retVal;
        }
        public void FlushAllDatabase()
        {
            IServer server = _redisCacheDatabaseProvider.GetServer();
            server.FlushAllDatabases();
        }

        public void KeyDeleteWithPrefix(string prefix)
        {
            _database = _redisCacheDatabaseProvider.GetDatabase();
            prefix = $"{prefix.ToUpper()}*";
            _database.ScriptEvaluate(@"
                local keys = redis.call('keys', ARGV[1]) 
                for i=1,#keys,5000 do 
                redis.call('del', unpack(keys, i, math.min(i+4999, #keys)))
                end", values: new RedisValue[] { prefix });
        }
    }
}
