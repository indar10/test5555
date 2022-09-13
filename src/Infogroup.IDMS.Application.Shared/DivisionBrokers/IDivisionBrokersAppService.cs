using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.DivisionBrokers.Dtos;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.DivisionBrokers
{
    public interface IDivisionBrokersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetDivisionBrokerForViewDto>> GetAll(GetAllDivisionBrokersInput input);

		Task<GetDivisionBrokerForEditOutput> GetDivisionBrokerForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditDivisionBrokerDto input);

		Task Delete(EntityDto input);

		
    }
}