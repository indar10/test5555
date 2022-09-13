using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Databases;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.UserDatabases.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.UserDatabases
{
    [AbpAuthorize]
    public class UserDatabasesAppService : IDMSAppServiceBase, IUserDatabasesAppService
    {
        private readonly IRepository<UserDatabase> _userDatabaseRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<Database, int> _lookup_databaseRepository;


        public UserDatabasesAppService(IRepository<UserDatabase> userDatabaseRepository, IRepository<User, long> lookup_userRepository, IRepository<Database, int> lookup_databaseRepository)
        {
            _userDatabaseRepository = userDatabaseRepository;
            _lookup_userRepository = lookup_userRepository;
            _lookup_databaseRepository = lookup_databaseRepository;

        }

        public async Task<PagedResultDto<GetUserDatabaseForViewDto>> GetAll(GetAllUserDatabasesInput input)
        {

            var filteredUserDatabases = _userDatabaseRepository.GetAll()
                        .Include(e => e.UserFk)
                        .Include(e => e.DatabaseFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DatabasecDatabaseNameFilter), e => e.DatabaseFk != null && e.DatabaseFk.cDatabaseName.ToLower() == input.DatabasecDatabaseNameFilter.ToLower().Trim());

            var pagedAndFilteredUserDatabases = filteredUserDatabases
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var userDatabases = from o in pagedAndFilteredUserDatabases
                                join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                                from s1 in j1.DefaultIfEmpty()

                                join o2 in _lookup_databaseRepository.GetAll() on o.DatabaseId equals o2.Id into j2
                                from s2 in j2.DefaultIfEmpty()

                                select new GetUserDatabaseForViewDto()
                                {
                                    UserDatabase = new UserDatabaseDto
                                    {
                                        Id = o.Id
                                    },
                                    UserName = s1 == null ? "" : s1.Name.ToString(),
                                    DatabasecDatabaseName = s2 == null ? "" : s2.cDatabaseName.ToString()
                                };

            var totalCount = await filteredUserDatabases.CountAsync();

            return new PagedResultDto<GetUserDatabaseForViewDto>(
                totalCount,
                await userDatabases.ToListAsync()
            );
        }

        [AbpAuthorize]
        public async Task<GetUserDatabaseForEditOutput> GetUserDatabaseForEdit(EntityDto input)
        {
            var userDatabase = await _userDatabaseRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetUserDatabaseForEditOutput { UserDatabase = ObjectMapper.Map<CreateOrEditUserDatabaseDto>(userDatabase) };

            var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.UserDatabase.UserId);
            output.UserName = _lookupUser.Name.ToString();
            var _lookupDatabase = await _lookup_databaseRepository.FirstOrDefaultAsync((int)output.UserDatabase.DatabaseId);
            output.DatabasecDatabaseName = _lookupDatabase.cDatabaseName.ToString();

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditUserDatabaseDto input)
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

        [AbpAuthorize]
        protected virtual async Task Create(CreateOrEditUserDatabaseDto input)
        {
            var userDatabase = ObjectMapper.Map<UserDatabase>(input);



            await _userDatabaseRepository.InsertAsync(userDatabase);
        }

        [AbpAuthorize]
        protected virtual async Task Update(CreateOrEditUserDatabaseDto input)
        {
            var userDatabase = await _userDatabaseRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, userDatabase);
        }

        [AbpAuthorize]
        public async Task Delete(EntityDto input)
        {
            await _userDatabaseRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize]
        public async Task<PagedResultDto<UserDatabaseUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_userRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<UserDatabaseUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new UserDatabaseUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.Name?.ToString()
                });
            }

            return new PagedResultDto<UserDatabaseUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize]
        public async Task<PagedResultDto<UserDatabaseDatabaseLookupTableDto>> GetAllDatabaseForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_databaseRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.cDatabaseName.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var databaseList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<UserDatabaseDatabaseLookupTableDto>();
            foreach (var database in databaseList)
            {
                lookupTableDtoList.Add(new UserDatabaseDatabaseLookupTableDto
                {
                    Id = database.Id,
                    DisplayName = database.cDatabaseName?.ToString()
                });
            }

            return new PagedResultDto<UserDatabaseDatabaseLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}