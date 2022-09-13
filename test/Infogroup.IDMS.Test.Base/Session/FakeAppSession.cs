using Infogroup.IDMS.Sessions;

namespace Infogroup.IDMS.Test.Base.Session
{
    public class FakeAppSession : IAppSession
    {
        public string IDMSUserEmail => "IDMSDevNotifications@infogroup.com";
        public string IDMSUserFullName => "Saarthak Pande";
        public int IDMSUserId => 3176;
        public string IDMSUserName => "saArthakp";
    }
}
