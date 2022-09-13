using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.States
{
    public interface IRedisStateCache
    {
        int GetDatabase(int databaseId);
        List<DropdownOutputDto> GetState(int databaseId, string databaseType);
        List<DropdownOutputDto> GetCounty(string cStateCode, int databaseId);
        List<DropdownOutputDto> GetCity(string cStateCode, string cCountyCode, int databaseId);
        List<DropdownOutputDto> GetNeighborhood(string cStateCode, int databaseId, string city);
    }
}