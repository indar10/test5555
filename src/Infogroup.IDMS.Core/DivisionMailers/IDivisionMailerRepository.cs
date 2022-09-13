using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.DivisionMailers.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.DivisionMailers
{
    public interface IDivisionMailerRepository : IRepository<DivisionMailer, int>
    {
        PagedResultDto<GetDivisionMailerForViewDto> GetAllDivisionMailerList(Tuple<string, string, List<SqlParameter>> query);
        
    }
}
