using Abp.Domain.Repositories;
using Infogroup.IDMS.Caching;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infogroup.IDMS.Builds
{
    public class RedisBuildCache : IRedisBuildCache
    {
        private readonly IRepository<Build, int> _buildRepository;
        private readonly IRedisCacheHelper _redisHelper;
        private readonly string keyPrefix = "BUILD";

        public RedisBuildCache(
            IRepository<Build, int> buildRepository,
            IRedisCacheHelper redisHelper)
        {
            _buildRepository = buildRepository;
            _redisHelper = redisHelper;
        }

        public BuildCacheItem GetBuild(int id)
        {
            BuildCacheItem buildCacheItem;
            try
            {
                var key = $"{keyPrefix}_{id}";
                var buildCache = _redisHelper.GetString(key);
                if (buildCache == null)
                {
                    buildCacheItem = FetchBuildItem(id);
                    if (buildCacheItem != null)
                    {
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(buildCacheItem));
                    }
                }
                else
                    buildCacheItem = JsonConvert.DeserializeObject<BuildCacheItem>(buildCache);
            }
            catch (RedisConnectionException)
            {
                buildCacheItem = FetchBuildItem(id);
            }
            return buildCacheItem;
        }

        private BuildCacheItem FetchBuildItem(int id)
        {
            BuildCacheItem cacheItem = null;
            var build = _buildRepository.Get(id);
            if (build != null)
            {
                cacheItem = new BuildCacheItem
                {
                    cBuild = build.cBuild,
                    cDescription = build.cDescription,
                    iRecordCount = build.iRecordCount,
                    iIsReadyToUse = build.iIsReadyToUse,
                    iIsOnDisk = build.iIsOnDisk,
                    DatabaseId = build.DatabaseId
                };
            }
            return cacheItem;
        }
    }
}