using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.Databases.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Databases
{
    public class DatabaseRepository : IDMSRepositoryBase<Database, int>, IDatabaseRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public DatabaseRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public DatabaseDto GetDataSetDatabaseByOrderID(int iOrderID)
        {
            var result = new DatabaseDto();
            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand(@"Select tblDatabase.ID, tblDatabase.LK_DatabaseType ,tblDatabase.DivisionID from tblDatabase inner join tblBuild
            on tblBuild.DatabaseID = tblDatabase.ID inner join tblOrder on tblOrder.BuildID = tblBuild.ID and  tblOrder.ID = " + iOrderID, CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        return new DatabaseDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                            DivisionId = Convert.ToInt32((dataReader["DivisionID"])),
                            LK_DatabaseType = dataReader["LK_DatabaseType"].ToString()
                        };
                    }

                }
                return result;
            }
        }

        public DatabaseDto GetDatabaseIDByOrderID(int iOrderID)
        {
            var result = new DatabaseDto();
            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand(@"Select build.DatabaseID as DatabaseID FROM tblBuild
             build WITH(NOLOCK) inner join tblOrder tblOrder WITH(NOLOCK) ON build.ID = tblOrder.BuildID WHERE tblOrder.ID =  " + iOrderID, CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        return new DatabaseDto
                        {
                            Id = Convert.ToInt32(dataReader["DatabaseID"]),
                        };
                    }

                }
                return result;
            }
        }

        public List<DropdownOutputDto> FetchDatabasesWithAccess(int iUserID)
        {
            var result = new List<DropdownOutputDto>();
            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand(@"SELECT tblDatabase.ID,tblDatabase.cDatabaseName  FROM tblUserDatabase
            LEFT JOIN tblDatabase 
            ON tblUserDatabase.DatabaseID = tblDatabase.ID
            LEFT JOIN (Select DatabaseID,UserID FROM tblUserDatabaseAccessObject where AccessObjectID = 141  and (iListAccess = 1 OR iAddEditAccess = 1)) DAO 
            ON DAO.DatabaseID = tblDatabase.ID and DAO.UserID =" + iUserID + 
            "WHERE tblUserDatabase.UserID = "+iUserID + 
            "AND DAO.DatabaseID IS NULL", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var dropDown = new DropdownOutputDto
                        {
                            Label = $"{dataReader["cDatabaseName"]} : {Convert.ToInt32(dataReader["ID"])}",
                            Value = Convert.ToInt32(dataReader["ID"]),
                        };
                        result.Add(dropDown);
                    }
                }
                return result;
            }
        }
        public List<DropdownOutputDto> GetDatabaseData(string Query, List<SqlParameter> sqlParameters)
        {
            _databaseHelper.EnsureConnectionOpen();
            var DbNames = new List<DropdownOutputDto>();
            using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        DbNames.Add(new DropdownOutputDto { Label = dataReader["cDatabaseName"].ToString(), Value = Convert.ToInt32(dataReader["ID"]) });
                    }
                }
            }

            return DbNames;
        }
    }
}
