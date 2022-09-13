namespace Infogroup.IDMS.Sessions
{
    public interface IAppSession
    {
        string IDMSUserEmail { get; }
        string IDMSUserFullName { get; }
        int IDMSUserId { get; }
        string IDMSUserName { get; }
    }
}