using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.SICCodeRelateds.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.SICCodeRelateds
{
    public interface ISICCodeRelatedsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSICCodeRelatedForViewDto>> GetAll(GetAllSICCodeRelatedsInput input);

		Task<GetSICCodeRelatedForEditOutput> GetSICCodeRelatedForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSICCodeRelatedDto input);

		Task Delete(EntityDto input);

		
    }
}