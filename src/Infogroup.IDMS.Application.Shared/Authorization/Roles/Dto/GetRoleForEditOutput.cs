using System.Collections.Generic;
using Infogroup.IDMS.Authorization.Permissions.Dto;

namespace Infogroup.IDMS.Authorization.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto Role { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}