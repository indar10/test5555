using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.SegmentLists.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.SegmentLists
{
    public class SegmentListRepository : IDMSRepositoryBase<SegmentList,int>,ISegmentListRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public SegmentListRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper) : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public async Task AddSourcesAsync(int id, string selectedListIDs, string sInitiatedBy, bool isSubSelect)
        {
            try
            {   var idParamaterName = "@SegmentID";
                var storedProcedureName = "usp_SegmentSelectionLists";                
                if (isSubSelect)
                {
                    idParamaterName = "@SubSelectID";
                    storedProcedureName = "usp_SubSelectSelectionLists"; 
                }
                _databaseHelper.EnsureConnectionOpen();
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter(idParamaterName, id),
                    new SqlParameter("@SelectionList", selectedListIDs),
                    new SqlParameter("@InitiatedBy", sInitiatedBy)
                };
                using (var command = _databaseHelper.CreateCommand(storedProcedureName, CommandType.StoredProcedure, sqlParameters.ToArray()))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<SourceDto>> GetApprovedSourcesAsync(Tuple<string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = new List<SourceDto>();
            try
            {
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new SourceDto
                            {
                                Id = 0,
                                ListID = Convert.ToInt32(dataReader["ID"]),
                                ListName = (dataReader["cListName"]).ToString(),
                                Action = ActionType.Add,
                            });
                        }
                    }
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task DeleteAsync(int iSegmentID, string selectedListIDs)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var query = $@"DELETE FROM tblSegmentList
                           WHERE SegmentID = {iSegmentID}
                           AND MasterLOLID IN({selectedListIDs})";

                using (var command = _databaseHelper.CreateCommand(query, CommandType.Text))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}