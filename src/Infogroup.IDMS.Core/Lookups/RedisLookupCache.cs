using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Caching;
using Infogroup.IDMS.ExternalBuildTableDatabases;
using Infogroup.IDMS.Shared.Dtos;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Infogroup.IDMS.Lookups
{
    public class RedisLookupCache : IRedisLookupCache
    {
        private readonly IRepository<Lookup, int> _lookupRepository;
        private readonly IRedisCacheHelper _redisHelper;
        private readonly IRepository<ExternalBuildTableDatabase> _externalBuildTableDatabaseRepository;
        private readonly string keyPrefix = "LKP";

        public RedisLookupCache(
            IRepository<Lookup, int> lookupRepository,
            IRepository<ExternalBuildTableDatabase> externalBuildTableDatabaseRepository,
            IRedisCacheHelper redisHelper)
        {
            _lookupRepository = lookupRepository;
            _redisHelper = redisHelper;
            _externalBuildTableDatabaseRepository = externalBuildTableDatabaseRepository;
        }

        public List<LookupCacheItem> GetLookUpFields(string cLookupValue, string cCode)
        {
            List<LookupCacheItem> lookupCacheItem = null;
            try
            {
                if (!string.IsNullOrEmpty(cLookupValue))
                {
                    var cCodePart = !string.IsNullOrEmpty(cCode) ? $"_{cCode}" : string.Empty;
                    var key = $"{keyPrefix}_{cLookupValue}{cCodePart}";
                    var lookupCache = _redisHelper.GetString(key);
                    if (lookupCache == null)
                    {
                        lookupCacheItem = FetchLookup(cLookupValue, cCode);
                        if (lookupCacheItem.Count > 0)
                        {
                            _redisHelper.SetString(key, JsonConvert.SerializeObject(lookupCacheItem));
                        }
                    }
                    else
                        lookupCacheItem = JsonConvert.DeserializeObject<List<LookupCacheItem>>(lookupCache);
                }
            }
            catch (RedisConnectionException)
            {
                lookupCacheItem = FetchLookup(cLookupValue, cCode);
            }
            return lookupCacheItem;
        }
        public List<DropdownOutputDto> GetXTabExternalFields(int databaseId)
        {
            List<DropdownOutputDto> lookupCacheItem;
            try
            {
                var key = $"{keyPrefix}_XTABEXTERNAL_DB_{databaseId}";
                var lookupCache = _redisHelper.GetString(key);
                if (lookupCache == null)
                {
                    lookupCacheItem = FetchXTabExternalFields(databaseId);
                    if (lookupCacheItem.Count > 0)
                    {
                        _redisHelper.SetString(key, JsonConvert.SerializeObject(lookupCacheItem));
                    }
                }
                else
                    lookupCacheItem = JsonConvert.DeserializeObject<List<DropdownOutputDto>>(lookupCache);
            }
            catch (RedisConnectionException)
            {
                lookupCacheItem = FetchXTabExternalFields(databaseId);
            }
            return lookupCacheItem;
        }

        private List<LookupCacheItem> FetchLookup(string cLookupValue, string cCode)
        {
            return _lookupRepository.GetAll()
                .WhereIf(!string.IsNullOrEmpty(cCode), lookup => lookup.cCode == cCode)
                .Where(lookup => lookup.cLookupValue.ToUpper() == cLookupValue && lookup.iIsActive)
                .OrderBy(lookup => lookup.iOrderBy)
                .Select(lookop => new LookupCacheItem
                {
                    cCode = lookop.cCode,
                    cDescription = lookop.cDescription,
                    cField = lookop.cField,
                    iField = lookop.iField,
                    mField = lookop.mField
                }).ToList();
        }

        private List<DropdownOutputDto> FetchXTabExternalFields(int databaseId)
        {
            var cLookupValue = "XTABEXTERNAL";
            var externalBuildTableIDs = _externalBuildTableDatabaseRepository.GetAll()
                .Where(q => q.DatabaseID.Equals(databaseId))
                .Select(exdb => exdb.BuildTableID.ToString()).ToList();

            return _lookupRepository.GetAll()
                .Where(lookup => externalBuildTableIDs.Contains(lookup.cCode) && lookup.cLookupValue.ToUpper() == cLookupValue && lookup.iIsActive)
                .Select(xtab => new DropdownOutputDto { Label = xtab.cDescription, Value = xtab.cField })
                .ToList();
        }
    }
}