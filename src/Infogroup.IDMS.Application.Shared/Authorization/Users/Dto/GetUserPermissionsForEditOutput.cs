using System.Collections.Generic;
using Infogroup.IDMS.Authorization.Permissions.Dto;

namespace Infogroup.IDMS.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}