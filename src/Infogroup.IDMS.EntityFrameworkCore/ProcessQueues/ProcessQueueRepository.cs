using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.ProcessQueues.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.ProcessQueues
{
  public  class ProcessQueueRepository: IDMSRepositoryBase<ProcessQueue, int>, IProcessQueueRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public ProcessQueueRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public PagedResultDto<ProcessQueueDto> GetAllProcessQueue(Tuple<string, string, List<SqlParameter>> query)

        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<ProcessQueueDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var LookupData = new List<ProcessQueueDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            LookupData.Add(new ProcessQueueDto
                            {
                                
                                Id= Convert.ToInt32(dataReader["ID"]),
                                cQueueName=dataReader["cQueueName"].ToString().Trim(),
                                cDescription=dataReader["cDescription"].ToString().Trim(),
                                iAllowedThreadCount= Convert.ToInt32(dataReader["iAllowedThreadCount"]),
                                iIsSuspended= Convert.ToBoolean(dataReader["iIsSuspended"]),
                                
                                LK_ProcessType=dataReader["LK_ProcessType"].ToString().Trim(),
                                LK_QueueType=dataReader["LK_QueueType"].ToString().Trim(),
                                cCreatedBy=dataReader["cCreatedBy"].ToString().Trim(),
                                dCreatedDate=Convert.ToDateTime(dataReader["dCreatedDate"]),
                                queueDescription= dataReader["QueueDescription"].ToString().Trim(),
                                processDescription= dataReader["ProcessDescription"].ToString().Trim(),

                            });
                        }
                    }
                    result.Items = LookupData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
       public PagedResultDto<ProcessQueueDatabaseDtoForView> GetAllDbData(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<ProcessQueueDatabaseDtoForView>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var LookupData = new List<ProcessQueueDatabaseDtoForView>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            LookupData.Add(new ProcessQueueDatabaseDtoForView
                            {

                                databaseId = Convert.ToInt32(dataReader["ID"]),
                                cDatabaseName = dataReader["cDatabaseName"].ToString().Trim(),
                                processQueueId = Convert.ToInt32(dataReader["PQID"]),

                            });
                        }
                    }
                    result.Items = LookupData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public List<DropdownOutputDto> GetLookupData(string Query, List<SqlParameter> sqlParameters)
        {
            _databaseHelper.EnsureConnectionOpen();
            var lookupNames = new List<DropdownOutputDto>();
            using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        lookupNames.Add(new DropdownOutputDto { Label = dataReader["cDescription"].ToString(), Value = dataReader["cCode"].ToString() });
                    }
                }
            }

            return lookupNames;
        }
        public int GetID(int databaseID, int PQID)
        {
            _databaseHelper.EnsureConnectionOpen();

            using (var command = _databaseHelper.CreateCommand($@" Select  BL.ID FROM tblProcessQueueDatabase BL WITH(NOLOCK)
                                                                WHERE BL.databaseId={databaseID} and BL.processQueueID={PQID}" , CommandType.Text))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
        public List<DropdownOutputDto> GetLookupDataForProcess(string Query, List<SqlParameter> sqlParameters)
        {
            _databaseHelper.EnsureConnectionOpen();
            var lookupNames = new List<DropdownOutputDto>();
            using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        lookupNames.Add(new DropdownOutputDto { Label = dataReader["cDescription"].ToString(), Value = dataReader["cCode"].ToString() });
                    }
                }
            }

            return lookupNames;
        }
       
    }
}
