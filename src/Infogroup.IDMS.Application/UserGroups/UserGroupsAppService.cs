using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.UserGroups.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.IDMSUsers;

namespace Infogroup.IDMS.UserGroups
{
    [AbpAuthorize(AppPermissions.Pages_UserGroups)]
    public class UserGroupsAppService : IDMSAppServiceBase, IUserGroupsAppService
    {
        private readonly IRepository<UserGroup> _userGroupRepository;
        private readonly IRepository<IDMSUser, int> _userRepository;


        public UserGroupsAppService(IRepository<UserGroup> userGroupRepository, IRepository<IDMSUser, int> userRepository)
        {
            _userGroupRepository = userGroupRepository;
            _userRepository = userRepository;

        }

        public async Task<PagedResultDto<GetUserGroupForViewDto>> GetAll(GetAllUserGroupsInput input)
        {

            var filteredUserGroups = _userGroupRepository.GetAll()
                        .Include(e => e.UserFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TblUsercFirstNameFilter), e => e.UserFk != null && e.UserFk.cFirstName.ToLower() == input.TblUsercFirstNameFilter.ToLower().Trim());

            var pagedAndFilteredUserGroups = filteredUserGroups
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var userGroups = from o in pagedAndFilteredUserGroups
                             join o1 in _userRepository.GetAll() on o.UserId equals o1.Id into j1
                             from s1 in j1.DefaultIfEmpty()

                             select new GetUserGroupForViewDto()
                             {
                                 UserGroup = new UserGroupDto
                                 {
                                     Id = o.Id
                                 },
                                 TblUsercFirstName = s1 == null ? "" : s1.cFirstName.ToString()
                             };

            var totalCount = await filteredUserGroups.CountAsync();

            return new PagedResultDto<GetUserGroupForViewDto>(
                totalCount,
                await userGroups.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_UserGroups_Edit)]
        public async Task<GetUserGroupForEditOutput> GetUserGroupForEdit(EntityDto input)
        {
            var userGroup = await _userGroupRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetUserGroupForEditOutput { UserGroup = ObjectMapper.Map<CreateOrEditUserGroupDto>(userGroup) };


            var _lookupTblUser = await _userRepository.FirstOrDefaultAsync((int)output.UserGroup.UserId);
            output.TblUsercFirstName = _lookupTblUser.cFirstName.ToString();

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditUserGroupDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_UserGroups_Create)]
        protected virtual async Task Create(CreateOrEditUserGroupDto input)
        {
            var userGroup = ObjectMapper.Map<UserGroup>(input);



            await _userGroupRepository.InsertAsync(userGroup);
        }

        [AbpAuthorize(AppPermissions.Pages_UserGroups_Edit)]
        protected virtual async Task Update(CreateOrEditUserGroupDto input)
        {
            var userGroup = await _userGroupRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, userGroup);
        }

        [AbpAuthorize(AppPermissions.Pages_UserGroups_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _userGroupRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_UserGroups)]
        public async Task<PagedResultDto<UserGroupTblUserLookupTableDto>> GetAllTblUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _userRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.cFirstName.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var tblUserList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<UserGroupTblUserLookupTableDto>();
            foreach (var tblUser in tblUserList)
            {
                lookupTableDtoList.Add(new UserGroupTblUserLookupTableDto
                {
                    Id = tblUser.Id,
                    DisplayName = tblUser.cFirstName?.ToString()
                });
            }

            return new PagedResultDto<UserGroupTblUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}