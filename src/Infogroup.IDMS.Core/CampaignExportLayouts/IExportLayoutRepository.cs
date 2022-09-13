using Abp.Domain.Repositories;
using Infogroup.IDMS.CampaignExportLayouts;
using Infogroup.IDMS.ExportLayouts.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infogroup.IDMS.CampaignExportLayouts
{
    public interface IExportLayoutRepository : IRepository<CampaignExportLayout, int>
    {
       
        void ReorderExportLayoutOrderId(int id, int orderId, string modifiedBy);
    }
}
