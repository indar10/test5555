using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.UI;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.BatchQueues.Dtos;
using System.Data.SqlClient;
using System.Data;
using Infogroup.IDMS.Databases;

namespace Infogroup.IDMS.BatchQueues
{
    public class BatchQueueRepository : IDMSRepositoryBase<BatchQueue, int>, IBatchQueueRepository
    {

        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;

        public BatchQueueRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public PagedResultDto<BatchQueueDto> GetAllBatchQueues(Tuple<string, string, List<SqlParameter>> query)
        {

            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<BatchQueueDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var queueData = new List<BatchQueueDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var DatabaseName = dataReader["DatabaseName"].ToString().Trim();
                            if (!string.IsNullOrEmpty(DatabaseName) && (DatabaseName.ToLower().StartsWith(DatabaseNameConst.Infogroup) || DatabaseName.ToLower().EndsWith(DatabaseNameConst.Database)))
                            {
                                DatabaseName = DatabaseName.Replace(DatabaseNameConst.Database, "", StringComparison.OrdinalIgnoreCase);
                                DatabaseName = DatabaseName.Replace(DatabaseNameConst.Infogroup, "", StringComparison.OrdinalIgnoreCase);
                            }
                            queueData.Add(new BatchQueueDto
                            {
                                ID = Convert.ToInt32(dataReader["QueueId"]),
                                iStatusId = Convert.ToInt32(dataReader["iStatusId"]),
                                FieldName = dataReader["FieldName"].ToString().Trim(),
                                ParmData = dataReader["ParmData"].ToString().Trim(),
                                cScheduleBy = dataReader["cScheduledBy"].ToString().Trim(),
                                dScheduled = Convert.ToDateTime(dataReader["dScheduled"]),
                                BuildDescription = dataReader["Build"].ToString().Trim(),
                                ProcessTypeDescription = dataReader["ProcessTypeDescription"].ToString().Trim(),
                                ProcessTypeId = Convert.ToInt32(dataReader["ProcessTypeId"]),
                                StatusDescription = dataReader["Status"].ToString().Trim(),
                                DataBaseName = DatabaseName,
                                Result = dataReader["Result"].ToString().Trim(),
                                duration = dataReader["dDuration"].ToString().Trim()
                            }); 
                        }
                    }
                    result.Items = queueData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public CreateOrEditBatchQueueDto GetQueueById(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new CreateOrEditBatchQueueDto();

            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result = new CreateOrEditBatchQueueDto
                        {
                            Id = Convert.ToInt32(dataReader["QueueId"]),
                            ProcessTypeDescription = dataReader["ProcessTypeDescription"].ToString().Trim(),
                            ProcessTypeId = Convert.ToInt32(dataReader["ProcessTypeId"]),
                            Result = dataReader["Result"].ToString().Trim(),
                            iStatusId = Convert.ToInt32(dataReader["iStatusId"]),
                        };
                    }
                }
            }
            return result;
        }     
    }
}
