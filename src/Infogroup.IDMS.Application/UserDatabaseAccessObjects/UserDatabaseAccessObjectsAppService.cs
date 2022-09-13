using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.AccessObjects;
using Infogroup.IDMS.Databases;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.UserDatabaseAccessObjects.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.UserDatabaseAccessObjects
{
    [AbpAuthorize]
	public class UserDatabaseAccessObjectsAppService : IDMSAppServiceBase, IUserDatabaseAccessObjectsAppService
    {
		 private readonly IRepository<UserDatabaseAccessObject> _userDatabaseAccessObjectRepository;
		 private readonly IRepository<IDMSUser,int> _lookup_idmsUserRepository;
		 private readonly IRepository<AccessObject,int> _lookup_accessObjectRepository;
		 private readonly IRepository<Database,int> _lookup_databaseRepository;
		 

		  public UserDatabaseAccessObjectsAppService(IRepository<UserDatabaseAccessObject> userDatabaseAccessObjectRepository , IRepository<IDMSUser, int> lookup_idmsUserRepository, IRepository<AccessObject, int> lookup_accessObjectRepository, IRepository<Database, int> lookup_databaseRepository) 
		  {
			_userDatabaseAccessObjectRepository = userDatabaseAccessObjectRepository;
			_lookup_idmsUserRepository = lookup_idmsUserRepository;
		_lookup_accessObjectRepository = lookup_accessObjectRepository;
		_lookup_databaseRepository = lookup_databaseRepository;
		
		  }

		 public async Task<PagedResultDto<GetUserDatabaseAccessObjectForViewDto>> GetAll(GetAllUserDatabaseAccessObjectsInput input)
         {
			
			var filteredUserDatabaseAccessObjects = _userDatabaseAccessObjectRepository.GetAll()
						.Include( e => e.IDMSUserFk)
						.Include( e => e.AccessObjectFk)
						.Include( e => e.DatabaseFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.IDMSUsercFirstNameFilter), e => e.IDMSUserFk != null && e.IDMSUserFk.cFirstName == input.IDMSUsercFirstNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AccessObjectcCodeFilter), e => e.AccessObjectFk != null && e.AccessObjectFk.cCode == input.AccessObjectcCodeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DatabasecDatabaseNameFilter), e => e.DatabaseFk != null && e.DatabaseFk.cDatabaseName == input.DatabasecDatabaseNameFilter);

			var pagedAndFilteredUserDatabaseAccessObjects = filteredUserDatabaseAccessObjects
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var userDatabaseAccessObjects = from o in pagedAndFilteredUserDatabaseAccessObjects
                         join o1 in _lookup_idmsUserRepository.GetAll() on o.IDMSUserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_accessObjectRepository.GetAll() on o.AccessObjectId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_databaseRepository.GetAll() on o.DatabaseId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         select new GetUserDatabaseAccessObjectForViewDto() {
							UserDatabaseAccessObject = new UserDatabaseAccessObjectDto
							{
                                Id = o.Id
							},
                         	IDMSUsercFirstName = s1 == null ? "" : s1.cFirstName.ToString(),
                         	AccessObjectcCode = s2 == null ? "" : s2.cCode.ToString(),
                         	DatabasecDatabaseName = s3 == null ? "" : s3.cDatabaseName.ToString()
						};

            var totalCount = await filteredUserDatabaseAccessObjects.CountAsync();

            return new PagedResultDto<GetUserDatabaseAccessObjectForViewDto>(
                totalCount,
                await userDatabaseAccessObjects.ToListAsync()
            );
         }
		 
		 public async Task<GetUserDatabaseAccessObjectForEditOutput> GetUserDatabaseAccessObjectForEdit(EntityDto input)
         {
            var userDatabaseAccessObject = await _userDatabaseAccessObjectRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserDatabaseAccessObjectForEditOutput {UserDatabaseAccessObject = ObjectMapper.Map<CreateOrEditUserDatabaseAccessObjectDto>(userDatabaseAccessObject)};

		    
                var _lookupIDMSUser = await _lookup_idmsUserRepository.FirstOrDefaultAsync((int)output.UserDatabaseAccessObject.IDMSUserId);
                output.IDMSUsercFirstName = _lookupIDMSUser.cFirstName.ToString();

		    
                var _lookupAccessObject = await _lookup_accessObjectRepository.FirstOrDefaultAsync((int)output.UserDatabaseAccessObject.AccessObjectId);
                output.AccessObjectcCode = _lookupAccessObject.cCode.ToString();

		    
                var _lookupDatabase = await _lookup_databaseRepository.FirstOrDefaultAsync((int)output.UserDatabaseAccessObject.DatabaseId);
                output.DatabasecDatabaseName = _lookupDatabase.cDatabaseName.ToString();
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUserDatabaseAccessObjectDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditUserDatabaseAccessObjectDto input)
         {
            var userDatabaseAccessObject = ObjectMapper.Map<UserDatabaseAccessObject>(input);

			

            await _userDatabaseAccessObjectRepository.InsertAsync(userDatabaseAccessObject);
         }

		 protected virtual async Task Update(CreateOrEditUserDatabaseAccessObjectDto input)
         {
            var userDatabaseAccessObject = await _userDatabaseAccessObjectRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, userDatabaseAccessObject);
         }

		 public async Task Delete(EntityDto input)
         {
            await _userDatabaseAccessObjectRepository.DeleteAsync(input.Id);
         } 

		public async Task<PagedResultDto<UserDatabaseAccessObjectIDMSUserLookupTableDto>> GetAllIDMSUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_idmsUserRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.cFirstName != null ? e.cFirstName.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var idmsUserList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserDatabaseAccessObjectIDMSUserLookupTableDto>();
			foreach(var idmsUser in idmsUserList){
				lookupTableDtoList.Add(new UserDatabaseAccessObjectIDMSUserLookupTableDto
				{
					Id = idmsUser.Id,
					DisplayName = idmsUser.cFirstName?.ToString()
				});
			}

            return new PagedResultDto<UserDatabaseAccessObjectIDMSUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		public async Task<PagedResultDto<UserDatabaseAccessObjectAccessObjectLookupTableDto>> GetAllAccessObjectForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_accessObjectRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.cCode != null ? e.cCode.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var accessObjectList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserDatabaseAccessObjectAccessObjectLookupTableDto>();
			foreach(var accessObject in accessObjectList){
				lookupTableDtoList.Add(new UserDatabaseAccessObjectAccessObjectLookupTableDto
				{
					Id = accessObject.Id,
					DisplayName = accessObject.cCode?.ToString()
				});
			}

            return new PagedResultDto<UserDatabaseAccessObjectAccessObjectLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		public async Task<PagedResultDto<UserDatabaseAccessObjectDatabaseLookupTableDto>> GetAllDatabaseForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_databaseRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.cDatabaseName != null ? e.cDatabaseName.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var databaseList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserDatabaseAccessObjectDatabaseLookupTableDto>();
			foreach(var database in databaseList){
				lookupTableDtoList.Add(new UserDatabaseAccessObjectDatabaseLookupTableDto
				{
					Id = database.Id,
					DisplayName = database.cDatabaseName?.ToString()
				});
			}

            return new PagedResultDto<UserDatabaseAccessObjectDatabaseLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}