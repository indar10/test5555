using Infogroup.IDMS.Authorization.Users;

namespace Infogroup.IDMS.Authorization
{
    public interface IPasswordManager
    {
        void CheckRecentPasswords(User user, string newPassword);

        string GetLastPasswordChange(string userName);
    }
}