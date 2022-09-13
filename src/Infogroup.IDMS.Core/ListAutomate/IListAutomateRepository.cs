using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ListAutomate.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.ListAutomate
{
    public interface IListAutomateRepository : IRepository<ListAutomates, int>
    {
        PagedResultDto<IListAutomateDto> GetAllListAutomate(Tuple<string, string, List<SqlParameter>> query);
        List<DropdownOutputDto> GetConversionFrequency(Tuple<string, string, List<SqlParameter>> query);
        
    }
}
