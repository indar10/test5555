
namespace Infogroup.IDMS.Builds
{
    public class BuildCacheItem
    {
        public string cBuild { get; set; }
        public string cDescription { get; set; }
        public int iRecordCount { get; set; }
        public bool iIsReadyToUse { get; set; }
        public bool iIsOnDisk { get; set; }
        public int? DatabaseId { get; set; }
    }
}