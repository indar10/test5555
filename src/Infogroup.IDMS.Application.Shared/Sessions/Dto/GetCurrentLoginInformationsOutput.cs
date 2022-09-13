using Infogroup.IDMS.UiCustomization.Dto;

namespace Infogroup.IDMS.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutput
    {
        public UserLoginInfoDto User { get; set; }

        public TenantLoginInfoDto Tenant { get; set; }
        
        public ApplicationInfoDto Application { get; set; }
        public UiCustomizationSettingsDto Theme { get; set; }
        public IdmsUserLoginInfoDto IdmsUser { get; set; }
    }
}