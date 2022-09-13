using System.Collections.Generic;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Infogroup.IDMS.Authorization;
using Infogroup.IDMS.Lookups;
using System.Linq;
using Infogroup.IDMS.Redis;
using StackExchange.Redis;
using System;

namespace Infogroup.IDMS.Caching
{
    [AbpAuthorize(AppPermissions.Pages_RedisCache)]
    public class RedisCachingAppService : IDMSAppServiceBase, IRedisCachingAppService
    {
        private readonly IRedisCacheHelper _redisHelper;
        private readonly IRedisLookupCache _lookUpCache;
        private readonly IRepository<Lookup> _lookupRepository;

        public RedisCachingAppService(IRedisCacheHelper redisHelper,
            IRepository<Lookup> lookupRepository,
            IRedisLookupCache lookUpCache)
        {
            _redisHelper = redisHelper;
            _lookupRepository = lookupRepository;
            _lookUpCache = lookUpCache;
        }

        public List<RedisCacheDto> GetAll()
        {
            try
            {
                var allCacheList = _lookUpCache.GetLookUpFields("REDISCACHEKEYS")
                                   .Select(lookup => new RedisCacheDto
                                    {
                                        Value = lookup.cCode,
                                        Label = lookup.cDescription
                                    }).ToList();
                GetCacheCount(allCacheList);
                return allCacheList;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public void ClearCache(string keyPrefix)
        {
            try
            {
                _redisHelper.KeyDeleteWithPrefix(keyPrefix);
            }
            catch (RedisConnectionException )
            {
                throw new UserFriendlyException(this.L("RedisConnectionError"));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        public void ClearAllCaches()
        {
            try
            {
                _redisHelper.FlushAllDatabase();
            }
            catch (RedisConnectionException)
            {
                throw new UserFriendlyException(this.L("RedisConnectionError"));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        private void GetCacheCount(List<RedisCacheDto> cacheItems)
        {
            try
            {
                cacheItems.ForEach(cacheItem =>
                {
                    cacheItem.Count = _redisHelper.KeyCountByPrefix(cacheItem.Value).ToString();
                });
            }
            catch(RedisConnectionException )
            {
                cacheItems.ForEach(cacheItem =>
                {
                    cacheItem.Count = L("ServiceDownMsg");
                });
            }
        }
    }
}