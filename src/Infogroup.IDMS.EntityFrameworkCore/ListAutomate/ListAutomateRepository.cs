using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.ListAutomate;
using Infogroup.IDMS.ListAutomate.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.ListAutomate
{
    public class ListAutomateRepository: IDMSRepositoryBase<ListAutomates, int>,IListAutomateRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public ListAutomateRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public PagedResultDto<IListAutomateDto> GetAllListAutomate(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new PagedResultDto<IListAutomateDto>();


            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var listAutomateata = new List<IListAutomateDto>();
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        listAutomateata.Add(new IListAutomateDto
                        {
                            Id = Convert.ToInt32(dataReader["Id"]),
                            ListId = Convert.ToInt32(dataReader["ListId"]),
                            BuildId = Convert.ToInt32(dataReader["BuildId"]),
                            LK_ListConversionFrequency = dataReader["cDescription"].ToString(),
                            iInterval = Convert.ToInt32(dataReader["iInterval"]),
                            cScheduleTime = dataReader["cScheduleTime"].ToString(),
                            cSystemFileNameReadyToLoad = dataReader["cSystemFileNameReadyToLoad"].ToString(),
                            iIsActive = Convert.ToBoolean(dataReader["iIsActive"].ToString())
                        });
                    }
                }
                result.Items = listAutomateata;
            }
            return result;
        }        

        public List<DropdownOutputDto> GetConversionFrequency(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();
           var frequencyList = new List<DropdownOutputDto>();
            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                var TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        frequencyList.Add(new DropdownOutputDto
                        {
                            Value = dataReader["cCode"].ToString(),
                            Label = dataReader["cDescription"].ToString()
                        }) ;
                        
                    }
                }
            }
            return frequencyList;

        }
    }
}
