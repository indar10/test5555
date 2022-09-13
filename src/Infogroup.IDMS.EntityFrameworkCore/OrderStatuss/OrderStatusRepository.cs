using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.SavedSelections.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infogroup.IDMS.OrderStatuss
{
    public class OrderStatusRepository: IDMSRepositoryBase<OrderStatus, int>, IOrderStatusRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public OrderStatusRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public int GetOrderStatus(int campaignID)
        {
            _databaseHelper.EnsureConnectionOpen();
            int result = 0;
            using (var command = _databaseHelper.CreateCommand($@" Select iStatus from tblOrderStatus with (nolock) where orderid = {campaignID} and iIsCurrent = 1", CommandType.Text))
            {
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }
    }
}
