using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Brokers.Dtos;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Brokers
{
    public interface IBrokerRepository: IRepository<Broker, int>
    {
        PagedResultDto<BrokersDto> GetAllBrokersList(string input1, string input2, List<SqlParameter> sqlParameters, string companyOrCode);        
    }
}
