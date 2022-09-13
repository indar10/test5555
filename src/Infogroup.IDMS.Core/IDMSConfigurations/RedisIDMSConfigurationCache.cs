using Abp.Domain.Repositories;
using Abp.UI;
using Infogroup.IDMS.Caching;
using Infogroup.IDMS.Common;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;

namespace Infogroup.IDMS.IDMSConfigurations
{
    public class RedisIDMSConfigurationCache : IRedisIDMSConfigurationCache
    {
        private readonly IRepository<IDMSConfiguration, int> _configurationRepository;
        private readonly IRedisCacheHelper _redisHelper;
        private readonly Utils _utils;
        private const string keyPrefix = "CFG";

        public RedisIDMSConfigurationCache(
            IRepository<IDMSConfiguration> configurationRepository,
            Utils utils,
            IRedisCacheHelper redisHelper)
        {
            _configurationRepository = configurationRepository;
            _utils = utils;
            _redisHelper = redisHelper;
        }

        public IDMSConfigurationCacheItem GetConfigurationValue(string cItem, int databaseId)
        {
            try
            {
                var configItem = GetConfigurationItem(cItem, databaseId);
                if (configItem == null && databaseId != 0) configItem = GetConfigurationItem(cItem);
                if (configItem == null)
                    throw new UserFriendlyException($"{cItem} configuration item not found.");
                if (configItem.iIsEncrypted)
                    configItem.cValue = _utils.Decrypt(configItem.cValue);
                return configItem;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IDMSConfigurationCacheItem GetConfigurationItem(string cItem, int databaseId = 0)
        {
            IDMSConfigurationCacheItem configurationDto = null;
            try
            {
                if (!string.IsNullOrEmpty(cItem))
                {
                    var key = $"{keyPrefix}_{databaseId}_{cItem}";
                    var configurationCache = _redisHelper.GetString(key);
                    if (configurationCache == null)
                    {
                        configurationDto = FetchConfiguration(cItem, databaseId);
                        if (configurationDto != null)
                        {
                            _redisHelper.SetString(key, JsonConvert.SerializeObject(configurationDto));
                        }
                    }
                    else
                        configurationDto = JsonConvert.DeserializeObject<IDMSConfigurationCacheItem>(configurationCache);
                }
            }
            catch (RedisConnectionException)
            {
                configurationDto = FetchConfiguration(cItem, databaseId);
            }
            return configurationDto;
        }

        public bool IsAWSConfigured(int databaseId)
        {
            bool isConfigured;
            try
            {
                var key = $"{keyPrefix}_{databaseId}_AWS";
                var configurationCache = _redisHelper.GetString(key);
                if (configurationCache == null)
                {
                    isConfigured = CheckAWSFlag(databaseId);
                    _redisHelper.SetString(key, JsonConvert.SerializeObject(isConfigured));
                }
                else
                    isConfigured = JsonConvert.DeserializeObject<bool>(configurationCache);
            }
            catch (RedisConnectionException)
            {
                isConfigured = CheckAWSFlag(databaseId);
            }
            return isConfigured;
        }

        private IDMSConfigurationCacheItem FetchConfiguration(string cItem, int databaseId)
        {
            return _configurationRepository.GetAll()
                 .Where(x => x.cItem == cItem && x.DatabaseID == databaseId && x.iIsActive)
                 .OrderByDescending(x => x.Id)
                 .Select(config => new IDMSConfigurationCacheItem
                 {
                     cValue = config.cValue,
                     mValue = config.mValue,
                     iValue = config.iValue,
                     iIsEncrypted = config.iIsEncrypted
                 })
                .FirstOrDefault();
        }

        private bool CheckAWSFlag(int databaseId)
        {
            var configItem = _configurationRepository.GetAll()
                 .Where(x => x.cItem == "AWS" && x.cDescription == "CountProcess" && x.iIsActive && (x.DatabaseID == databaseId || x.DatabaseID == 0))
                 .OrderByDescending(x => x.DatabaseID)
                 .Select(config => config)
                .FirstOrDefault();
            if (configItem == null)
                throw new UserFriendlyException("AWS configuration item not found.");
            return configItem.cValue.Trim() == "1";
        }
    }
}

