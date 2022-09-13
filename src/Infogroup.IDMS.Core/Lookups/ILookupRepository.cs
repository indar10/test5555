using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Lookups.Dtos;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Lookups
{
    public interface ILookupRepository : IRepository<Lookup, int>
    {
        PagedResultDto<LookupDto> GetAllLookupList(Tuple<string, string, List<SqlParameter>> query);

        CreateOrEditLookupDto GetLookupById(Tuple<string, string, List<SqlParameter>> query);

        List<DropdownOutputDto> GetLookupData(string Query, List<SqlParameter> sqlParameters);
    }
}
