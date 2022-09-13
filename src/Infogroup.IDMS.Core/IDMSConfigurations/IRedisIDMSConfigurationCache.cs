namespace Infogroup.IDMS.IDMSConfigurations
{
    public interface IRedisIDMSConfigurationCache
    {
        IDMSConfigurationCacheItem GetConfigurationValue(string cItem, int databaseId = 0);
        bool IsAWSConfigured(int databaseId);
    }
}
