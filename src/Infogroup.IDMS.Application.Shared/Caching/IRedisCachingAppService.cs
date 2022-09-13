using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.Redis;

namespace Infogroup.IDMS.Caching
{
    public interface IRedisCachingAppService : IApplicationService
    {
        List<RedisCacheDto> GetAll();
        void ClearCache(string cacheKey);
        void ClearAllCaches();
    }
}
