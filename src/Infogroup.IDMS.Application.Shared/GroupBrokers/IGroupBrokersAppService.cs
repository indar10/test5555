using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.GroupBrokers.Dtos;
using Infogroup.IDMS.Dto;


namespace Infogroup.IDMS.GroupBrokers
{
    public interface IGroupBrokersAppService : IApplicationService
    {
        PagedResultDto<GroupBrokerDto> GetAllGroupBroker(GetAllGroupBrokersInputDto input);

        PagedResultDto<GroupBrokerDto> GetAllBroker(GetAllBrokersInputDto input);
    }
}