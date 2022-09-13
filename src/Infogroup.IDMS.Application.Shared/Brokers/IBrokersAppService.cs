using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Brokers.Dtos;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Brokers
{
    public interface IBrokersAppService : IApplicationService
    {
        PagedResultDto<BrokersDto> GetAllBrokers(GetAllBrokersInput input);
        Task<CreateOrEditBrokerDto> GetBrokerForEdit(EntityDto input);
        Task CreateOrEdit(CreateOrEditBrokerDto input);
    }
}