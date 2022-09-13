using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;
namespace Infogroup.IDMS.Lookups
{
    public interface IRedisLookupCache
    {
        List<LookupCacheItem> GetLookUpFields(string cLookupValue, string cCode = "");
        List<DropdownOutputDto> GetXTabExternalFields(int databaseId);
    }
}
