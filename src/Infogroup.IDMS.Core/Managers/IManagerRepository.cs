using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Managers.Dtos;
using Infogroup.IDMS.MasterLoLs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Managers
{
    public interface IManagerRepository : IRepository<Manager, int>
    {
        PagedResultDto<ManagerDto> GetAllManagersList(Tuple<string, string, List<SqlParameter>> query);

        List<ContactAssignmentsDto> GetContactAssignmentsData(string query, List<SqlParameter> sqlParameters);      
       
    }
}
