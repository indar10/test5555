

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.SysSendMails.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infogroup.IDMS.SysSendMails
{
	[AbpAuthorize]
    public class SysSendMailsAppService : IDMSAppServiceBase, ISysSendMailsAppService
    {
		 private readonly IRepository<SysSendMail> _sysSendMailRepository;
		 

		  public SysSendMailsAppService(IRepository<SysSendMail> sysSendMailRepository ) 
		  {
			_sysSendMailRepository = sysSendMailRepository;
			
		  }

		 public async Task<PagedResultDto<GetSysSendMailForViewDto>> GetAll(GetAllSysSendMailsInput input)
         {
			
			var filteredSysSendMails = _sysSendMailRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.cRecipients.Contains(input.Filter) || e.cMessage.Contains(input.Filter) || e.cFrom.Contains(input.Filter) || e.cReplyTo.Contains(input.Filter) || e.cCopyRecipients.Contains(input.Filter) || e.cBlindCopyRecipients.Contains(input.Filter) || e.cSubject.Contains(input.Filter) || e.cFileName.Contains(input.Filter) || e.cFailureMessage.Contains(input.Filter) || e.cCreatedBy.Contains(input.Filter));

			var pagedAndFilteredSysSendMails = filteredSysSendMails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var sysSendMails = from o in pagedAndFilteredSysSendMails
                         select new GetSysSendMailForViewDto() {
							SysSendMail = new SysSendMailDto
							{
                                Id = o.Id
							}
						};

            var totalCount = await filteredSysSendMails.CountAsync();

            return new PagedResultDto<GetSysSendMailForViewDto>(
                totalCount,
                await sysSendMails.ToListAsync()
            );
         }
		 
		 public async Task<GetSysSendMailForEditOutput> GetSysSendMailForEdit(EntityDto input)
         {
            var sysSendMail = await _sysSendMailRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetSysSendMailForEditOutput {SysSendMail = ObjectMapper.Map<CreateOrEditSysSendMailDto>(sysSendMail)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditSysSendMailDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditSysSendMailDto input)
         {
            var sysSendMail = ObjectMapper.Map<SysSendMail>(input);

			

            await _sysSendMailRepository.InsertAsync(sysSendMail);
         }

		 protected virtual async Task Update(CreateOrEditSysSendMailDto input)
         {
            var sysSendMail = await _sysSendMailRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, sysSendMail);
         }

		 public async Task Delete(EntityDto input)
         {
            await _sysSendMailRepository.DeleteAsync(input.Id);
         } 
    }
}