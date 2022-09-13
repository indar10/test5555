using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.OrderExportParts.Dtos;
using Infogroup.IDMS.Dto;
using System.Collections.Generic;

namespace Infogroup.IDMS.OrderExportParts
{
    public interface IOrderExportPartsAppService : IApplicationService 
    {
        

		void Delete(int campaignId);
        void Insert(List<EditCampaignExportPartDto> editCampaignExportPartList, string userName);





    }
}