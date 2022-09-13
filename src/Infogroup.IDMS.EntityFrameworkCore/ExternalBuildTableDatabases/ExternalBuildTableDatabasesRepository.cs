using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.ExternalBuildTableDatabases.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.ExternalBuildTableDatabases
{
   public class ExternalBuildTableDatabasesRepository : IDMSRepositoryBase<ExternalBuildTableDatabase, int>, IExternalBuildTableDatabasesRepository
    {

        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public ExternalBuildTableDatabasesRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public PagedResultDto<ExternalBuildTableDatabaseForAllDto> GetAllExternaldbLinks(Tuple<string, string, List<SqlParameter>> query)

        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<ExternalBuildTableDatabaseForAllDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var dbLinks = new List<ExternalBuildTableDatabaseForAllDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            dbLinks.Add(new ExternalBuildTableDatabaseForAllDto
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                DatabaseID= Convert.ToInt32(dataReader["DatabaseID"]),
                                BuildTableID = Convert.ToInt32(dataReader["BuildTableID"]),
                                DivisionID = Convert.ToInt32(dataReader["DivisionID"]),
                                BuildTableDescription= dataReader["BuildTableDescription"].ToString().Trim(),
                                DatabaseName= dataReader["DatabaseName"].ToString().Trim(),
                                DivisionName= dataReader["DivisionName"].ToString().Trim(),
                                BuildTableName= dataReader["BuildTableName"].ToString().Trim(),



                            }) ;
                        }
                    }
                    result.Items = dbLinks;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }
        public List<DropdownOutputDto> GetTableDescription(string Query)
        {
            _databaseHelper.EnsureConnectionOpen();
            var database = new List<DropdownOutputDto>();
            using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        database.Add(new DropdownOutputDto { Label = dataReader["cTableDescription"].ToString(), Value = Convert.ToInt32(dataReader["ID"]) });
                    }
                }
            }

            return database;
        }
    

   }
}
