using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.GroupBrokers.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.GroupBrokers
{
    public interface IGroupBrokerRepository : IRepository<GroupBroker, int>
    {
        PagedResultDto<GroupBrokerDto> GetAllGroupBrokersList(Tuple<string, string, List<SqlParameter>> query);

        PagedResultDto<GroupBrokerDto> GetAllBrokersList(Tuple<string, string, List<SqlParameter>> query);

        void DeleteBroker(int GroupID);
    }
}
