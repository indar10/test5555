using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Offers
{
    public class OfferRepository : IDMSRepositoryBase<Offer, int>, IOfferRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public OfferRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public void UpdateCASApproval(string status, string notes, string ipAddress, string userName, int? offerid)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@Status", status));
            sqlParameters.Add(new SqlParameter("@Notes", notes));
            sqlParameters.Add(new SqlParameter("@IpAddress", ipAddress));
            sqlParameters.Add(new SqlParameter("@UserName", userName));
            sqlParameters.Add(new SqlParameter("@OfferID", offerid));

            using (var command = _databaseHelper.CreateCommand("usp_UpdateCASApproval", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
