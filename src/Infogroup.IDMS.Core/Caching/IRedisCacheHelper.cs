
namespace Infogroup.IDMS.Caching
{
    public interface IRedisCacheHelper
    {
        void SetString(string key, string value, bool setExpiration = true);
        string GetString(string key);
        void KeyDeleteWithPrefix(string prefix);
        int KeyCountByPrefix(string prefix);
        void FlushAllDatabase();
    }
}
