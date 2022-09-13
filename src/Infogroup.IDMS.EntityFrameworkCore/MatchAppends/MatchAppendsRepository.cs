using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.BuildTableLayouts;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.MatchAppends.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.MatchAppends
{
    public class MatchAppendsRepository : IDMSRepositoryBase<MatchAppend, int>, IMatchAppendsRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public MatchAppendsRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public PagedResultDto<GetMatchAppendForViewDto> GetAllMatchAppendTasksList(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new PagedResultDto<GetMatchAppendForViewDto>();


            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var modelData = new List<GetMatchAppendForViewDto>();
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var DatabaseName = dataReader["cDatabaseName"].ToString().Trim();
                        if (!string.IsNullOrEmpty(DatabaseName) && (DatabaseName.ToLower().StartsWith(DatabaseNameConst.Infogroup) || DatabaseName.ToLower().EndsWith(DatabaseNameConst.Database)))
                        {
                            DatabaseName = DatabaseName.Replace(DatabaseNameConst.Database, "", StringComparison.OrdinalIgnoreCase);
                            DatabaseName = DatabaseName.Replace(DatabaseNameConst.Infogroup, "", StringComparison.OrdinalIgnoreCase);
                        }
                        modelData.Add(new GetMatchAppendForViewDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                            DatabaseName = DatabaseName,
                            BuildDescription = dataReader["BuildAndDesc"].ToString(),
                            cClientName = dataReader["cClientName"].ToString(),
                            cRequestReason = dataReader["cRequestReason"].ToString(),
                            Status = $"{dataReader["iStatusID"].ToString()}: {dataReader["codeStatus"].ToString()}",
                            StatusId = Convert.ToInt32(dataReader["iStatusID"]),
                            StatusDate = Convert.ToDateTime(dataReader["StatusDate"]).ToString("MM/dd/yyyy"),
                            IDMSMatchFieldName = dataReader["cIDMSMatchFieldName"].ToString(),
                            cClientFileName = dataReader["cClientFileName"].ToString()
                        }) ;
                    }
                }
                result.Items = modelData;
            }
            return result;
        }
        public void CopyMatchAppendTask(int matchAppendId, string userName)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@SourceId", matchAppendId));
            sqlParameters.Add(new SqlParameter("@UserId", userName));

            using (var command = _databaseHelper.CreateCommand("usp_CopyMatchAppend", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }
        }

        public List<DropdownOutputDto> GetAllDatabasesFromUserId(Tuple<string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new List<DropdownOutputDto>();
            result.Add(new DropdownOutputDto { Label = "Select...", Value = 0 });

            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new DropdownOutputDto
                        {
                            Value = Convert.ToInt32(dataReader["ID"]),
                            Label = $"{dataReader["cDatabaseName"].ToString()} : {dataReader["ID"].ToString()}"

                        });
                    }
                }

            }
            return result;
        }

        public List<MatchAndAppendStatusDto> GetMatchAppendStatusDetails(int matchAppendID)
        {
            _databaseHelper.EnsureConnectionOpen();

            using (var command = _databaseHelper.CreateCommand($@" select MA.ID, L.cDescription ,U.cFirstName + ' ' + U.cLastName as cCreatedby, MA.dCreatedDate 
                                                                    from tblMatchAppendStatus MA Inner join tblLookup L on MA.iStatusID = L.cCode AND cLookupValue = 'MATCHANDAPPENDSTATUS' Inner join tblUser U on U.cUserID = MA.cCreatedBy
                                                                    where MA.MatchAppendID = {matchAppendID} order by MA.ID desc", CommandType.Text))

            {
                var MatchAppendstatus = new List<MatchAndAppendStatusDto>();
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var Status = new MatchAndAppendStatusDto
                        {
                            Id= Convert.ToInt32(dataReader["ID"]),
                            StatusDescription = dataReader["cDescription"].ToString(),
                            cCreatedBy = dataReader["cCreatedBy"].ToString(),
                            dCreatedDate = dataReader["dCreatedDate"].ToString()
                        };

                        MatchAppendstatus.Add(Status);
                    }
                }

                return MatchAppendstatus;
            }
        }
        

        public List<DropdownOutputDto> GetMatchAppendAvailableFields(int tableId)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new List<DropdownOutputDto>();
            
            var query = $@"SELECT BTL.cFieldName, BTL.cFieldDescription, BTL.iDataLength FROM tblBuildTableLayout BTL  WHERE BTL.iAllowExport = 1 AND BTL.BuildTableID = {tableId} ORDER BY BTL.cFieldDescription";

            using (var command = _databaseHelper.CreateCommand(query, CommandType.Text))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new DropdownOutputDto
                        {
                            Value = dataReader["cFieldName"].ToString(),
                            Label = dataReader["cFieldDescription"].ToString()

                        });
                    }
                }

            }
            return result;
        }

        


    }
}
