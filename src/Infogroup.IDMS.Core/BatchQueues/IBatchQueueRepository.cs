using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.BatchQueues.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.BatchQueues
{
    public interface IBatchQueueRepository : IRepository<BatchQueue, int>
    {
        PagedResultDto<BatchQueueDto> GetAllBatchQueues(Tuple<string, string, List<SqlParameter>> query);

        CreateOrEditBatchQueueDto GetQueueById(Tuple<string, string, List<SqlParameter>> query);

    }
}
