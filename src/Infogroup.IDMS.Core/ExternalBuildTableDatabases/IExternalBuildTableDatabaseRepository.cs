using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ExternalBuildTableDatabases.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.ExternalBuildTableDatabases
{
   public interface IExternalBuildTableDatabasesRepository : IRepository<ExternalBuildTableDatabase, int>
    {
        PagedResultDto<ExternalBuildTableDatabaseForAllDto> GetAllExternaldbLinks(Tuple<string, string, List<SqlParameter>> query);
        List<DropdownOutputDto> GetTableDescription(string Query);
    }
}
