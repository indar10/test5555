using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Infogroup.IDMS.Owners.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.OrderStatuss
{
    public interface IOrderStatusRepository : IRepository<OrderStatus, int>
    {
        int GetOrderStatus(int campaignId);
    }
}
