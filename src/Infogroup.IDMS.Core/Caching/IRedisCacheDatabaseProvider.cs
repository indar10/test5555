using StackExchange.Redis;

namespace Infogroup.IDMS.Caching
{
    public interface IRedisCacheDatabaseProvider
    {
        IDatabase GetDatabase();
        IServer GetServer();
    }
}