using Abp.Authorization;

namespace Infogroup.IDMS.IDMSUsers
{
    public interface IIDMSPermissionChecker
    {
        bool IsGranted(int userId, int databaseId, PermissionList iId, int iAccessLevel);
        bool IsGranted(int userId, PermissionList iId, int iAccessLevel);
        string GetDisplayPermissionName(Permission input);
    }
}