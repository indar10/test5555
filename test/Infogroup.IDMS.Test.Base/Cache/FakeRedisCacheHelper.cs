using Infogroup.IDMS.Caching;

namespace Infogroup.IDMS.Test.Base.Cache
{
    public class FakeRedisCacheHelper : IRedisCacheHelper
    {
        public void FlushAllDatabase() { }
        public string GetString(string key) => null;
        public int KeyCountByPrefix(string prefix) => 0;
        public void KeyDeleteWithPrefix(string prefix) { }
        public void SetString(string key, string value, bool setExpiration = true) { }
    }
}
