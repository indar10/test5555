using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.SysSendMails.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.SysSendMails
{
    public interface ISysSendMailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSysSendMailForViewDto>> GetAll(GetAllSysSendMailsInput input);

		Task<GetSysSendMailForEditOutput> GetSysSendMailForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSysSendMailDto input);

		Task Delete(EntityDto input);

		
    }
}