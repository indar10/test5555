using Infogroup.IDMS.Campaigns;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Abp.Domain.Repositories;
using Infogroup.IDMS.OrderExportParts.Dtos;
using Abp.Authorization;
using Abp.UI;

namespace Infogroup.IDMS.OrderExportParts
{
    [AbpAuthorize]
    public class OrderExportPartsAppService : IDMSAppServiceBase, IOrderExportPartsAppService
    {
		 private readonly IRepository<OrderExportPart> _orderExportPartRepository;
		 private readonly IRepository<Campaign,int> _lookup_campaignRepository;
		 

		  public OrderExportPartsAppService(IRepository<OrderExportPart> orderExportPartRepository , IRepository<Campaign, int> lookup_campaignRepository) 
		  {
			_orderExportPartRepository = orderExportPartRepository;
			_lookup_campaignRepository = lookup_campaignRepository;
		
		  }	

		 public void  Insert(List<EditCampaignExportPartDto> editCampaignExportPartList, string userName)
         {            
            try
            {
                for (int i = 0; i < editCampaignExportPartList.Count; i++)
                {

                    CheckExportPartValidations(editCampaignExportPartList[i]);
                }                

                foreach (var campaignExportPart in editCampaignExportPartList)
                    {                     


                        for (int i = 0; i < campaignExportPart.iQuantity.Count; i++)
                        {
                        var exportPart = new OrderExportPart();
                        exportPart.OrderId = campaignExportPart.OrderId;
                            exportPart.cPartNo = (i + 1).ToString();
                            exportPart.SegmentID = campaignExportPart.SegmentID;
                            exportPart.iQuantity = Convert.ToInt32(campaignExportPart.iQuantity[i]);
                            exportPart.dCreatedDate = DateTime.Now;
                            exportPart.cCreatedBy = userName;
                            exportPart.dModifiedDate = null;
                            exportPart.cModifiedBy = null;
                            _orderExportPartRepository.Insert(exportPart);
                        }

                       

                    }
                    
                
            }catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
         }

        private void CheckExportPartValidations(EditCampaignExportPartDto campaignExportPart)
        {
            foreach (var item in campaignExportPart.iQuantity)
            {


                switch (item)
                {
                    case "":

                        throw new UserFriendlyException(L("BlankRowValues", campaignExportPart.iDedupeOrderSpecified));
                    case "0":

                        throw new UserFriendlyException(L("ZeroRowValues", campaignExportPart.iDedupeOrderSpecified));
                    default: break;
                }
            }
            var sumOfQuantity = campaignExportPart.iQuantity.Sum(x => Convert.ToInt32(x));
            if (sumOfQuantity != campaignExportPart.OutputQuantity)
            {
                throw new UserFriendlyException(L("SumValuesValidation", campaignExportPart.iDedupeOrderSpecified, campaignExportPart.OutputQuantity));
            }
        }
		 public void Delete(int campaignId)
         {
            try
            {
                _orderExportPartRepository.Delete(x => x.OrderId == campaignId);
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
         } 

		
    }
}