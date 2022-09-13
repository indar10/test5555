using Infogroup.IDMS.Divisions;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.UserDivisions.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.IDMSUsers;

namespace Infogroup.IDMS.UserDivisions
{
	[AbpAuthorize]
    public class UserDivisionsAppService : IDMSAppServiceBase, IUserDivisionsAppService
    {
		 private readonly IRepository<UserDivision> _userDivisionRepository;
		 private readonly IRepository<IDMSUser,int> _userRepository;
		 private readonly IRepository<Division,int> _lookup_divisionRepository;
		 

		  public UserDivisionsAppService(IRepository<UserDivision> userDivisionRepository , IRepository<IDMSUser, int> userRepository, IRepository<Division, int> lookup_divisionRepository) 
		  {
			_userDivisionRepository = userDivisionRepository;
            _userRepository = userRepository;
		    _lookup_divisionRepository = lookup_divisionRepository;
		
		  }

		 public async Task<PagedResultDto<GetUserDivisionForViewDto>> GetAll(GetAllUserDivisionsInput input)
         {
			
			var filteredUserDivisions = _userDivisionRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter));


			var query = (from o in filteredUserDivisions
                         join o1 in _userRepository.GetAll() on o.UserID equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_divisionRepository.GetAll() on o.DivisionID equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetUserDivisionForViewDto() {
							UserDivision = new UserDivisionDto
							{
                                iSelectedBuildID = o.iSelectedBuildID,
                                iSelectedDatabaseID = o.iSelectedDatabaseID,
                                cCreatedBy = o.cCreatedBy,
                                dCreatedDate = o.dCreatedDate,
                                cModifiedBy = o.cModifiedBy,
                                dModifiedDate = o.dModifiedDate,
                                Id = o.Id
							},
                         	tblUsercFirstName = s1 == null ? "" : s1.cFirstName.ToString(),
                         	DivisioncDivisionName = s2 == null ? "" : s2.cDivisionName.ToString()
						})
						.WhereIf(!string.IsNullOrWhiteSpace(input.tblUsercFirstNameFilter), e => e.tblUsercFirstName.ToLower() == input.tblUsercFirstNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DivisioncDivisionNameFilter), e => e.DivisioncDivisionName.ToLower() == input.DivisioncDivisionNameFilter.ToLower().Trim());

            var totalCount = await query.CountAsync();

            var userDivisions = await query
                .OrderBy(input.Sorting ?? "userDivision.id asc")
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<GetUserDivisionForViewDto>(
                totalCount,
                userDivisions
            );
         }
		 
		 public async Task<GetUserDivisionForEditOutput> GetUserDivisionForEdit(EntityDto input)
         {
            var userDivision = await _userDivisionRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserDivisionForEditOutput {UserDivision = ObjectMapper.Map<CreateOrEditUserDivisionDto>(userDivision)};

		    
                var _lookuptblUser = await _userRepository.FirstOrDefaultAsync((int)output.UserDivision.UserID);
                output.tblUsercFirstName = _lookuptblUser.cFirstName.ToString();
            
		    
                var _lookupDivision = await _lookup_divisionRepository.FirstOrDefaultAsync((int)output.UserDivision.DivisionID);
                output.DivisioncDivisionName = _lookupDivision.cDivisionName.ToString();
            
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUserDivisionDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 private async Task Create(CreateOrEditUserDivisionDto input)
         {
            var userDivision = ObjectMapper.Map<UserDivision>(input);

			

            await _userDivisionRepository.InsertAsync(userDivision);
         }

		 private async Task Update(CreateOrEditUserDivisionDto input)
         {
            var userDivision = await _userDivisionRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, userDivision);
         }

         public async Task Delete(EntityDto input)
         {
            await _userDivisionRepository.DeleteAsync(input.Id);
         } 

         public async Task<PagedResultDto<UserDivisiontblUserLookupTableDto>> GetAlltblUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _userRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cFirstName.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var tblUserList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserDivisiontblUserLookupTableDto>();
			foreach(var tblUser in tblUserList){
				lookupTableDtoList.Add(new UserDivisiontblUserLookupTableDto
				{
					Id = tblUser.Id,
					DisplayName = tblUser.cFirstName?.ToString()
				});
			}

            return new PagedResultDto<UserDivisiontblUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

         public async Task<PagedResultDto<UserDivisionDivisionLookupTableDto>> GetAllDivisionForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_divisionRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cDivisionName.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var divisionList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserDivisionDivisionLookupTableDto>();
			foreach(var division in divisionList){
				lookupTableDtoList.Add(new UserDivisionDivisionLookupTableDto
				{
					Id = division.Id,
					DisplayName = division.cDivisionName?.ToString()
				});
			}

            return new PagedResultDto<UserDivisionDivisionLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}