using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using System;
using System.Data;


namespace Infogroup.IDMS.IDMSUsers
{
    public class IDMSUserRepository : IDMSRepositoryBase<IDMSUser, int>, IIDMSUserRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public IDMSUserRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public IDMSUser GetUserByUserName(string userName)
        {
            var result = new IDMSUser();
            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand(@"select top 1 * from tblUser where cUserID='" + userName + "'", CommandType.Text))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Id = Convert.ToInt32(dataReader["ID"]);
                        result.cEmail = dataReader["cEmail"].ToString();
                        result.cFirstName = dataReader["cFirstName"].ToString();
                        result.cLastName = dataReader["cLastName"].ToString();
                        var MailerID = dataReader["MailerID"];
                        result.MailerID = (MailerID != System.DBNull.Value) ? Convert.ToInt32(MailerID) : 0;
                    }
                }
            }
            return result;
        }
    }
}
