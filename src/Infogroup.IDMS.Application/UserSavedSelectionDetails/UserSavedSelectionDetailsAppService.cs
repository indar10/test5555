using Infogroup.IDMS.UserSavedSelections;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.UserSavedSelectionDetails.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.UserSavedSelectionDetails
{
	[AbpAuthorize]
    public class UserSavedSelectionDetailsAppService : IDMSAppServiceBase, IUserSavedSelectionDetailsAppService
    {
		 private readonly IRepository<UserSavedSelectionDetail> _userSavedSelectionDetailRepository;
		 private readonly IRepository<UserSavedSelection,int> _lookup_userSavedSelectionRepository;
		 

		  public UserSavedSelectionDetailsAppService(IRepository<UserSavedSelectionDetail> userSavedSelectionDetailRepository , IRepository<UserSavedSelection, int> lookup_userSavedSelectionRepository) 
		  {
			_userSavedSelectionDetailRepository = userSavedSelectionDetailRepository;
			_lookup_userSavedSelectionRepository = lookup_userSavedSelectionRepository;
		
		  }

		 public async Task<PagedResultDto<GetUserSavedSelectionDetailForViewDto>> GetAll(GetAllUserSavedSelectionDetailsInput input)
         {
			
			var filteredUserSavedSelectionDetails = _userSavedSelectionDetailRepository.GetAll()
						.Include( e => e.UserSavedSelectionFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cQuestionFieldName.Contains(input.Filter) || e.cQuestionDescription.Contains(input.Filter) || e.cJoinOperator.Contains(input.Filter) || e.cGrouping.Contains(input.Filter) || e.cValues.Contains(input.Filter) || e.cValueMode.Contains(input.Filter) || e.cDescriptions.Contains(input.Filter) || e.cValueOperator.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter) || e.cTableName.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserSavedSelectioncDescriptionFilter), e => e.UserSavedSelectionFk != null && e.UserSavedSelectionFk.cDescription.ToLower() == input.UserSavedSelectioncDescriptionFilter.ToLower().Trim());

			var pagedAndFilteredUserSavedSelectionDetails = filteredUserSavedSelectionDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var userSavedSelectionDetails = from o in pagedAndFilteredUserSavedSelectionDetails
                         join o1 in _lookup_userSavedSelectionRepository.GetAll() on o.UserSavedSelectionId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetUserSavedSelectionDetailForViewDto {
							UserSavedSelectionDetail = new UserSavedSelectionDetailDto
							{
                                Id = o.Id
							},
                         	UserSavedSelectioncDescription = s1 == null ? "" : s1.cDescription.ToString()
						};

            var totalCount = await filteredUserSavedSelectionDetails.CountAsync();

            return new PagedResultDto<GetUserSavedSelectionDetailForViewDto>(
                totalCount,
                await userSavedSelectionDetails.ToListAsync()
            );
         }
		 
		 public async Task<GetUserSavedSelectionDetailForEditOutput> GetUserSavedSelectionDetailForEdit(EntityDto input)
         {
            var userSavedSelectionDetail = await _userSavedSelectionDetailRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserSavedSelectionDetailForEditOutput {UserSavedSelectionDetail = ObjectMapper.Map<CreateOrEditUserSavedSelectionDetailDto>(userSavedSelectionDetail)};

            var _lookupUserSavedSelection = await _lookup_userSavedSelectionRepository.FirstOrDefaultAsync((int)output.UserSavedSelectionDetail.UserSavedSelectionId);
            output.UserSavedSelectioncDescription = _lookupUserSavedSelection.cDescription.ToString();
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUserSavedSelectionDetailDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditUserSavedSelectionDetailDto input)
         {
            var userSavedSelectionDetail = ObjectMapper.Map<UserSavedSelectionDetail>(input);

			

            await _userSavedSelectionDetailRepository.InsertAsync(userSavedSelectionDetail);
         }

		 protected virtual async Task Update(CreateOrEditUserSavedSelectionDetailDto input)
         {
            var userSavedSelectionDetail = await _userSavedSelectionDetailRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, userSavedSelectionDetail);
         }

		 public async Task Delete(EntityDto input)
         {
            await _userSavedSelectionDetailRepository.DeleteAsync(input.Id);
         } 

		 public async Task<PagedResultDto<UserSavedSelectionDetailUserSavedSelectionLookupTableDto>> GetAllUserSavedSelectionForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userSavedSelectionRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cDescription.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userSavedSelectionList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserSavedSelectionDetailUserSavedSelectionLookupTableDto>();
			foreach(var userSavedSelection in userSavedSelectionList){
				lookupTableDtoList.Add(new UserSavedSelectionDetailUserSavedSelectionLookupTableDto
				{
					Id = userSavedSelection.Id,
					DisplayName = userSavedSelection.cDescription?.ToString()
				});
			}

            return new PagedResultDto<UserSavedSelectionDetailUserSavedSelectionLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}