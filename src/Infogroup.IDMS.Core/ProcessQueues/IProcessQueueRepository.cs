
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.ProcessQueues.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.ProcessQueues
{
   public interface IProcessQueueRepository : IRepository<ProcessQueue, int>
    {
        PagedResultDto<ProcessQueueDto> GetAllProcessQueue(Tuple<string, string, List<SqlParameter>> query);
        PagedResultDto<ProcessQueueDatabaseDtoForView> GetAllDbData(Tuple<string, string, List<SqlParameter>> query);
        List<DropdownOutputDto> GetLookupData(string Query, List<SqlParameter> sqlParameters);
        List<DropdownOutputDto> GetLookupDataForProcess(string Query, List<SqlParameter> sqlParameters);
        int GetID(int databaseID, int PQID);
    }
}
