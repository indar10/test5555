using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Sessions.Dto
{
    public class IdmsUserLoginInfoDto
    {
        public int IDMSUserID { get; set; }
        public string IDMSUserName { get; set; }
        public int UserMailerID { get; set; }
        public int CurrentCampaignDatabaseId { get; set; }
    }

}
