

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.IDMSUsers.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace Infogroup.IDMS.IDMSUsers
{
	[AbpAuthorize]
    public class IDMSUsersAppService : IDMSAppServiceBase, IIDMSUsersAppService
    {
		 private readonly IRepository<IDMSUser> _userRepository;
		 

		  public IDMSUsersAppService(IRepository<IDMSUser> userRepository) 
		  {
            _userRepository = userRepository;
			
		  }

		 public async Task<PagedResultDto<IDMSUserDto>> GetAll(GetAllIDMSUsersInput input)
         {
            try { 
			var filteredTblUsers = _userRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cFirstName.Contains(input.Filter) || e.cLastName.Contains(input.Filter) || e.cUserID.Contains(input.Filter) || e.cEmail.Contains(input.Filter) || e.cPhone.Contains(input.Filter) || e.cFax.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter) || e.cModifiedBy.Contains(input.Filter) || e.LK_AccountingDivisionCode.Contains(input.Filter));


			var query = (from o in filteredTblUsers
                         select new IDMSUserDto {
                                cFirstName = o.cFirstName,
                                cLastName = o.cLastName,
                                cUserID = o.cUserID,
                                cEmail = o.cEmail,
                                cPhone = o.cPhone,
                                cFax = o.cFax,
                                iIsActive = o.iIsActive,
                                iIsNotify = o.iIsNotify,
                                iLogonAttempts = o.iLogonAttempts,
                                LastLogonDate = o.LastLogonDate,
                                cCreatedBy = o.cCreatedBy,
                                dCreatedDate = o.dCreatedDate,
                                cModifiedBy = o.cModifiedBy,
                                dModifiedDate = o.dModifiedDate,
                                LK_AccountingDivisionCode = o.LK_AccountingDivisionCode,
                                //DivisionBrokerID = o.DivisionBrokerID,
                                //DivisionMailerID = o.DivisionMailerID,
                                //MailerID = o.MailerID,
                                Id = o.Id
                               
							}
						);

            var totalCount = await query.CountAsync();

            var tblUsers = await query
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<IDMSUserDto>(
                totalCount,
                tblUsers
            );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
		 
		 public async Task<GetIDMSUserForViewDto> GetTblUserForView(int id)
         {
            var tblUser = await _userRepository.GetAsync(id);

            var output = new GetIDMSUserForViewDto { TblUser = ObjectMapper.Map<IDMSUserDto>(tblUser) };
			
            return output;
         }
		 
		 public async Task<GetIDMSUserForEditOutput> GetTblUserForEdit(EntityDto input)
         {
            var tblUser = await _userRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetIDMSUserForEditOutput {TblUser = ObjectMapper.Map<CreateOrEditIDMSUserDto>(tblUser)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditIDMSUserDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 private async Task Create(CreateOrEditIDMSUserDto input)
         {
            var tblUser = ObjectMapper.Map<IDMSUser>(input);

			

            await _userRepository.InsertAsync(tblUser);
         }

		 private async Task Update(CreateOrEditIDMSUserDto input)
         {
            var tblUser = await _userRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, tblUser);
         }

         public async Task Delete(EntityDto input)
         {
            await _userRepository.DeleteAsync(input.Id);
         } 
    }
}