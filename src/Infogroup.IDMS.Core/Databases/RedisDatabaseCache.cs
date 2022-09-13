using Abp.Domain.Repositories;
using Infogroup.IDMS.Caching;
using StackExchange.Redis;

namespace Infogroup.IDMS.Databases
{
    public class RedisDatabaseCache : IRedisDatabaseCache
    {
        private readonly IRepository<Database, int> _databaseRepository;
        private readonly IRedisCacheHelper _redisHelper;
        private readonly string keyPrefix = "DB";

        public RedisDatabaseCache(
            IRepository<Database, int> databaseRepository,
            IRedisCacheHelper redisHelper)
        {
            _databaseRepository = databaseRepository;
            _redisHelper = redisHelper;
        }
      
        string IRedisDatabaseCache.GetDatabaseType(int Id)
        {
            var databaseType = string.Empty;
            try
            {
                if (Id > 0)
                {
                    var key = $"{keyPrefix}_TYPE_{Id}";
                    databaseType = _redisHelper.GetString(key);
                    if (databaseType == null)
                    {
                        databaseType = _databaseRepository.Get(Id).LK_DatabaseType;
                        if (databaseType != null)
                        {
                            _redisHelper.SetString(key, databaseType);
                        }
                    }
                    else
                        return databaseType;
                }
            }
            catch (RedisConnectionException)
            {
                return _databaseRepository.Get(Id).LK_DatabaseType;
            }
            return databaseType;
        }
    }
}