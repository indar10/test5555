using System.Collections.Generic;
using Infogroup.IDMS.CampaignFavourites.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.UserDatabaseMailers.Dtos;

namespace Infogroup.IDMS.IDMSUsers
{
    public interface IRedisIDMSUserCache
    {
        List<DropdownOutputDto> GetDropdownOptions(int iUserID, UserDropdown dropdownType);
        void SetDropdownOptions(int iUserID, UserDropdown dropdownType);
        List<int> GetDatabaseIDs(int iUserID);
        void SetDatabaseIDs(int iUserID);
        List<CampaignFavouriteDtoForView> GetCampaignFavourites(int iUserID);
        void SetCampaignFavourites(int iUserID);
        List<GetUserDatabaseMailerForViewDto> GetUserDatabaseMailerList(int iUserID);
        int GetDefaultDatabaseForCampaign(int userId, int abpUserId);
        void SetDefaultDatabaseForCampaign(int userId,int databaseId);
        List<UserDatabaseAccessObjectCacheItem> GetDatabaseAccessObjects(int iUserID);
        void SetDatabaseAccessObjects(int iUserID);
        List<UserAccessObjectCacheItem> GetAccessObjects(int iUserID);
        void SetAccessObjects(int iUserID);
    }
}