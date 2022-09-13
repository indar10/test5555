using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization.Permissions.Dto;

namespace Infogroup.IDMS.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
