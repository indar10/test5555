using Abp.Auditing;
using Infogroup.IDMS.Configuration.Dto;

namespace Infogroup.IDMS.Configuration.Tenants.Dto
{
    public class TenantEmailSettingsEditDto : EmailSettingsEditDto
    {
        public bool UseHostDefaultEmailSettings { get; set; }
    }
}