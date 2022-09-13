using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.CampaignExportLayouts;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.ExportLayouts.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.CampaignExportLayouts
{
    public class ExportLayoutRepository : IDMSRepositoryBase<CampaignExportLayout, int>, IExportLayoutRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public ExportLayoutRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        
        public void ReorderExportLayoutOrderId(int id, int orderId, string modifiedBy)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@OrderExportLayoutID", id));
            sqlParameters.Add(new SqlParameter("@MoveTo", orderId));
            sqlParameters.Add(new SqlParameter("@InitiatedBy", modifiedBy));


            using (var command = _databaseHelper.CreateCommand("usp_MoveOrderExportLayoutFields", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }
        }
       

    }
}
