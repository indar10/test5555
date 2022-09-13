using Abp.Authorization;
using Abp.Localization;
using Abp.UI;
using System;
using System.Linq;

namespace Infogroup.IDMS.IDMSUsers
{
    public class IDMSPermissionChecker : IDMSDomainServiceBase, IIDMSPermissionChecker
    {
        private readonly IRedisIDMSUserCache _userCache;
        private readonly ILocalizationContext _localizationContext;
        public IDMSPermissionChecker(IRedisIDMSUserCache userCache, ILocalizationContext localizationContext)
        {
            _userCache = userCache;
            _localizationContext = localizationContext;
        }
        public bool IsGranted(int userId,PermissionList iId, int iAccessLevel)
        {
            bool result = false;
            var userAccessObject = _userCache.GetAccessObjects(userId).FirstOrDefault(c =>  c.AccessObjectId == (int)iId);
            if (userAccessObject != null)
            {
                if (iAccessLevel == AccessLevel.iAddEdit)
                    result = userAccessObject.AddEditAccess;
                else
                    result = userAccessObject.ListAccess;
            }
            return result;
        }
        public bool IsGranted(int userId,int databaseId, PermissionList iId, int iAccessLevel)
        {
            try
            {
                var userDatabaseAccessObject = _userCache.GetDatabaseAccessObjects(userId).FirstOrDefault(c => ((c.DatabaseId == databaseId) && (c.AccessObjectId == (int)iId)));
                //If no entry found for selected DatabaseID and AccessObjectID then we need to carry regular operation.
                if (userDatabaseAccessObject != null)
                {
                    //1. If the list/view permission is revoked then as good as even Add/Edit also.
                    //2. if the add/edit permission is revoked and at the same time he is looking for add/edit
                    //3. This is when somebody inserted the entry into Database directly set the both the access level as 0
                    if ((userDatabaseAccessObject.ListAccess)
                        || (userDatabaseAccessObject.AddEditAccess && iAccessLevel == AccessLevel.iAddEdit))
                        return false;
                    else
                        return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public string GetDisplayPermissionName(Permission input)
        {
            if (input.Parent.Name.Equals("Pages"))
                return input.DisplayName.Localize(_localizationContext);

            return $@"{GetDisplayPermissionName(input.Parent)}.{input.DisplayName.Localize(_localizationContext)}";
        }
    }
}