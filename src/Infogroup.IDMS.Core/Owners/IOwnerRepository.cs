using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Owners.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.Owners
{
    public interface IOwnerRepository : IRepository<Owner, int>
    {
        PagedResultDto<OwnerDto> GetAllOwnersList(Tuple<string, string, List<SqlParameter>> query);     
    }
}
