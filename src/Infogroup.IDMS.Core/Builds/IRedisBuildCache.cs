namespace Infogroup.IDMS.Builds
{
    public interface IRedisBuildCache
    {
        BuildCacheItem GetBuild(int id);
    }
}