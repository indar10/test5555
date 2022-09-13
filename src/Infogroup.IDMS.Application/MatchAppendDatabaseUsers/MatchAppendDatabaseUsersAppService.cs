

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.MatchAppendDatabaseUsers.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Sessions;
using Abp.UI;

namespace Infogroup.IDMS.MatchAppendDatabaseUsers
{
	[AbpAuthorize]
    public class MatchAppendDatabaseUsersAppService : IDMSAppServiceBase, IMatchAppendDatabaseUsersAppService
    {
		private readonly AppSession _mySession;
		private readonly IRepository<MatchAppendDatabaseUser> _matchAppendDatabaseUserRepository;
		 

		  public MatchAppendDatabaseUsersAppService(IRepository<MatchAppendDatabaseUser> matchAppendDatabaseUserRepository,
			  AppSession mySession) 
		  {
			_matchAppendDatabaseUserRepository = matchAppendDatabaseUserRepository;
			_mySession = mySession;

		}

		 public async Task<PagedResultDto<GetMatchAppendDatabaseUserForViewDto>> GetAll(GetAllMatchAppendDatabaseUsersInput input)
         {
			
			var filteredMatchAppendDatabaseUsers = _matchAppendDatabaseUserRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cCreatedBy.Contains(input.Filter) || 
						e.cModifiedBy.Contains(input.Filter) || e.UserID == Convert.ToInt32(input.Filter));

			var pagedAndFilteredMatchAppendDatabaseUsers = filteredMatchAppendDatabaseUsers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var matchAppendDatabaseUsers = from o in pagedAndFilteredMatchAppendDatabaseUsers
										   select new GetMatchAppendDatabaseUserForViewDto() {
											   MatchAppendDatabaseUser = new MatchAppendDatabaseUserDto
											   {
												   Id = o.Id,
												   DatabaseID = o.DatabaseID,
												   UserID = o.UserID
												}
						};

            var totalCount = await filteredMatchAppendDatabaseUsers.CountAsync();

            return new PagedResultDto<GetMatchAppendDatabaseUserForViewDto>(
                totalCount,
                await matchAppendDatabaseUsers.ToListAsync()
            );
         }
		 
		 [AbpAuthorize]
		 public async Task<GetMatchAppendDatabaseUserForEditOutput> GetMatchAppendDatabaseUserForEdit(EntityDto input)
         {
            var matchAppendDatabaseUser = await _matchAppendDatabaseUserRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetMatchAppendDatabaseUserForEditOutput {MatchAppendDatabaseUser = ObjectMapper.Map<CreateOrEditMatchAppendDatabaseUserDto>(matchAppendDatabaseUser)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditMatchAppendDatabaseUserDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize]
		 protected virtual async Task Create(CreateOrEditMatchAppendDatabaseUserDto input)
         {
            try {
				input.cCreatedDate = DateTime.Now.ToString();
				input.cCreatedBy = _mySession.IDMSUserName;
				var matchAppendDatabaseUser = ObjectMapper.Map<MatchAppendDatabaseUser>(input);
				await _matchAppendDatabaseUserRepository.InsertAsync(matchAppendDatabaseUser);
			}
			catch (Exception ex)
			{
				throw new UserFriendlyException(ex.Message);
			}
		}

		 [AbpAuthorize]
		 protected virtual async Task Update(CreateOrEditMatchAppendDatabaseUserDto input)
         {
            var matchAppendDatabaseUser = await _matchAppendDatabaseUserRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, matchAppendDatabaseUser);
         }

		 [AbpAuthorize]
         public async Task Delete(EntityDto input)
         {
            await _matchAppendDatabaseUserRepository.DeleteAsync(input.Id);
         } 
    }
}