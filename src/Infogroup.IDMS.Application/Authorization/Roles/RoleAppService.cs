using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Zero.Configuration;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Authorization.Permissions;
using Infogroup.IDMS.Authorization.Permissions.Dto;
using Infogroup.IDMS.Authorization.Roles.Dto;
using Infogroup.IDMS.Authorization.Roles.Exporting;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.IDMSUsers;

namespace Infogroup.IDMS.Authorization.Roles
{
    /// <summary>
    /// Application service that is used by 'role management' page.
    /// </summary>
    [AbpAuthorize(AppPermissions.Pages_Administration_Roles)]
    public class RoleAppService : IDMSAppServiceBase, IRoleAppService
    {
        private readonly RoleManager _roleManager;
        private readonly IRoleManagementConfig _roleManagementConfig;
        private readonly IRoleListExcelExporter _roleListExcelExporter;
        private readonly IIDMSPermissionChecker _permissionChecker;

        public RoleAppService(
            RoleManager roleManager,
            IRoleManagementConfig roleManagementConfig,
            IRoleListExcelExporter roleListExcelExporter,
            IIDMSPermissionChecker permissionChecker)
        {
            _roleManager = roleManager;
            _roleManagementConfig = roleManagementConfig;
            _roleListExcelExporter = roleListExcelExporter;
            _permissionChecker = permissionChecker;
        }

        public async Task<ListResultDto<RoleListDto>> GetRoles(GetRolesInput input)
        {
            var query = _roleManager.Roles;

            if (input.Permissions != null && input.Permissions.Any(p => !string.IsNullOrEmpty(p)))
            {
                input.Permissions = input.Permissions.Where(p => !string.IsNullOrEmpty(p)).ToList();

                var staticRoleNames = _roleManagementConfig.StaticRoles.Where(
                    r => r.GrantAllPermissionsByDefault &&
                         r.Side == AbpSession.MultiTenancySide
                ).Select(r => r.RoleName).ToList();

                foreach (var permission in input.Permissions)
                {
                    query = query.Where(r =>
                        r.Permissions.Any(rp => rp.Name == permission)
                            ? r.Permissions.Any(rp => rp.Name == permission && rp.IsGranted)
                            : staticRoleNames.Contains(r.Name)
                    );
                }
            }

            var roles = await query.ToListAsync();

            return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Roles_Create, AppPermissions.Pages_Administration_Roles_Edit)]
        public async Task<GetRoleForEditOutput> GetRoleForEdit(NullableIdDto input)
        {
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = new Permission[0];
            RoleEditDto roleEditDto;

            if (input.Id.HasValue) //Editing existing role?
            {
                var role = await _roleManager.GetRoleByIdAsync(input.Id.Value);
                grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
                roleEditDto = ObjectMapper.Map<RoleEditDto>(role);
            }
            else
            {
                roleEditDto = new RoleEditDto();
            }

            return new GetRoleForEditOutput
            {
                Role = roleEditDto,
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        public async Task CreateOrUpdateRole(CreateOrUpdateRoleInput input)
        {
            if (input.Role.Id.HasValue)
            {
                await UpdateRoleAsync(input);
            }
            else
            {
                await CreateRoleAsync(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Roles_Delete)]
        public async Task DeleteRole(EntityDto input)
        {
            var role = await _roleManager.GetRoleByIdAsync(input.Id);

            var users = await UserManager.GetUsersInRoleAsync(role.Name);
            foreach (var user in users)
            {
                CheckErrors(await UserManager.RemoveFromRoleAsync(user, role.Name));
            }

            CheckErrors(await _roleManager.DeleteAsync(role));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Roles_Edit)]
        protected virtual async Task UpdateRoleAsync(CreateOrUpdateRoleInput input)
        {
            Debug.Assert(input.Role.Id != null, "input.Role.Id should be set.");

            var role = await _roleManager.GetRoleByIdAsync(input.Role.Id.Value);
            role.DisplayName = input.Role.DisplayName;
            role.IsDefault = input.Role.IsDefault;

            await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Roles_Create)]
        protected virtual async Task CreateRoleAsync(CreateOrUpdateRoleInput input)
        {
            var role = new Role(AbpSession.TenantId, input.Role.DisplayName) { IsDefault = input.Role.IsDefault };
            CheckErrors(await _roleManager.CreateAsync(role));
            await CurrentUnitOfWork.SaveChangesAsync(); //It's done to get Id of the role.
            await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
        }

        private async Task UpdateGrantedPermissionsAsync(Role role, List<string> grantedPermissionNames)
        {
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(grantedPermissionNames);
            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Roles_Print)]
        public async Task<FileDto> GetRolesToExcel(int RoleId, List<string> Permissions)
        {
            var roleList = new List<RoleReportDto>();
            if(RoleId != 0)
            {
                var role = await _roleManager.GetRoleByIdAsync(RoleId);
                var permission = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
                var permissionNames = permission.Where(x => !x.Name.Equals("Pages")).OrderBy(x => x.Name).ToList();
                var permissionList = new List<string>();

                foreach (var item in permissionNames)
                {
                    permissionList.Add(_permissionChecker.GetDisplayPermissionName(item));
                }
                permissionList.Sort();
                foreach (var item in permissionList)
                {
                    var roleWithPermission = new RoleReportDto
                    {
                        Role = role.DisplayName,
                        Permission = item
                    };
                    roleList.Add(roleWithPermission);
                }
            }
            else
            {
                var query = _roleManager.Roles;

                if (Permissions != null && Permissions.Any(p => !string.IsNullOrEmpty(p)))
                {
                    Permissions = Permissions.Where(p => !string.IsNullOrEmpty(p)).ToList();

                    var staticRoleNames = _roleManagementConfig.StaticRoles.Where(
                        r => r.GrantAllPermissionsByDefault &&
                             r.Side == AbpSession.MultiTenancySide
                    ).Select(r => r.RoleName).ToList();

                    foreach (var permission in Permissions)
                    {
                        query = query.Where(r =>
                            r.Permissions.Any(rp => rp.Name == permission)
                                ? r.Permissions.Any(rp => rp.Name == permission && rp.IsGranted)
                                : staticRoleNames.Contains(r.Name)
                        );
                    }
                }

                var roles = await query.ToListAsync();
                foreach (var role in roles)
                {
                    var permission = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
                    var permissionNames = permission.Where(x => !x.Name.Equals("Pages")).OrderBy(x => x.Name).ToList();
                    var permissionList = new List<string>();

                    foreach (var item in permissionNames)
                    {
                        permissionList.Add(_permissionChecker.GetDisplayPermissionName(item));
                    }
                    permissionList.Sort();
                    foreach (var item in permissionList)
                    {
                        var roleWithPermission = new RoleReportDto
                        {
                            Role = role.DisplayName,
                            Permission = item
                        };
                        roleList.Add(roleWithPermission);
                    }
                }                
            }            
            return _roleListExcelExporter.ExportToFile(roleList);
        }
    }
}
