using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.AccessObjects;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.UserAccessObjects.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.UserAccessObjects
{
    [AbpAuthorize]
	public class UserAccessObjectsAppService : IDMSAppServiceBase, IUserAccessObjectsAppService
    {
		 private readonly IRepository<UserAccessObject> _userAccessObjectRepository;
		 private readonly IRepository<IDMSUser,int> _lookup_idmsUserRepository;
		 private readonly IRepository<AccessObject,int> _lookup_accessObjectRepository;
		 

		  public UserAccessObjectsAppService(IRepository<UserAccessObject> userAccessObjectRepository , IRepository<IDMSUser, int> lookup_idmsUserRepository, IRepository<AccessObject, int> lookup_accessObjectRepository) 
		  {
			_userAccessObjectRepository = userAccessObjectRepository;
			_lookup_idmsUserRepository = lookup_idmsUserRepository;
		_lookup_accessObjectRepository = lookup_accessObjectRepository;
		
		  }

		 public async Task<PagedResultDto<GetUserAccessObjectForViewDto>> GetAll(GetAllUserAccessObjectsInput input)
         {
			
			var filteredUserAccessObjects = _userAccessObjectRepository.GetAll()
						.Include( e => e.IDMSUserFk)
						.Include( e => e.AccessObjectFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.IDMSUsercUserIDFilter), e => e.IDMSUserFk != null && e.IDMSUserFk.cUserID == input.IDMSUsercUserIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AccessObjectcCodeFilter), e => e.AccessObjectFk != null && e.AccessObjectFk.cCode == input.AccessObjectcCodeFilter);

			var pagedAndFilteredUserAccessObjects = filteredUserAccessObjects
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var userAccessObjects = from o in pagedAndFilteredUserAccessObjects
                         join o1 in _lookup_idmsUserRepository.GetAll() on o.IDMSUserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_accessObjectRepository.GetAll() on o.AccessObjectId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetUserAccessObjectForViewDto() {
							UserAccessObject = new UserAccessObjectDto
							{
                                Id = o.Id
							},
                         	IDMSUsercUserID = s1 == null ? "" : s1.cUserID.ToString(),
                         	AccessObjectcCode = s2 == null ? "" : s2.cCode.ToString()
						};

            var totalCount = await filteredUserAccessObjects.CountAsync();

            return new PagedResultDto<GetUserAccessObjectForViewDto>(
                totalCount,
                await userAccessObjects.ToListAsync()
            );
         }
		 
		 public async Task<GetUserAccessObjectForEditOutput> GetUserAccessObjectForEdit(EntityDto input)
         {
            var userAccessObject = await _userAccessObjectRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserAccessObjectForEditOutput {UserAccessObject = ObjectMapper.Map<CreateOrEditUserAccessObjectDto>(userAccessObject)};

		    
                var _lookupIDMSUser = await _lookup_idmsUserRepository.FirstOrDefaultAsync((int)output.UserAccessObject.IDMSUserId);
                output.IDMSUsercUserID = _lookupIDMSUser.cUserID.ToString();

		    
                var _lookupAccessObject = await _lookup_accessObjectRepository.FirstOrDefaultAsync((int)output.UserAccessObject.AccessObjectId);
                output.AccessObjectcCode = _lookupAccessObject.cCode.ToString();
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUserAccessObjectDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditUserAccessObjectDto input)
         {
            var userAccessObject = ObjectMapper.Map<UserAccessObject>(input);

			

            await _userAccessObjectRepository.InsertAsync(userAccessObject);
         }

		 protected virtual async Task Update(CreateOrEditUserAccessObjectDto input)
         {
            var userAccessObject = await _userAccessObjectRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, userAccessObject);
         }

		 public async Task Delete(EntityDto input)
         {
            await _userAccessObjectRepository.DeleteAsync(input.Id);
         } 

		public async Task<PagedResultDto<UserAccessObjectIDMSUserLookupTableDto>> GetAllIDMSUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_idmsUserRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.cUserID != null ? e.cUserID.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var idmsUserList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserAccessObjectIDMSUserLookupTableDto>();
			foreach(var idmsUser in idmsUserList){
				lookupTableDtoList.Add(new UserAccessObjectIDMSUserLookupTableDto
				{
					Id = idmsUser.Id,
					DisplayName = idmsUser.cUserID?.ToString()
				});
			}

            return new PagedResultDto<UserAccessObjectIDMSUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		public async Task<PagedResultDto<UserAccessObjectAccessObjectLookupTableDto>> GetAllAccessObjectForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_accessObjectRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.cCode != null ? e.cCode.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var accessObjectList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserAccessObjectAccessObjectLookupTableDto>();
			foreach(var accessObject in accessObjectList){
				lookupTableDtoList.Add(new UserAccessObjectAccessObjectLookupTableDto
				{
					Id = accessObject.Id,
					DisplayName = accessObject.cCode?.ToString()
				});
			}

            return new PagedResultDto<UserAccessObjectAccessObjectLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}