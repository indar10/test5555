using Abp.UI;
using Infogroup.IDMS.CampaignExportLayouts;
using Infogroup.IDMS.OrderStatuss;
using System;
using System.Linq;

namespace Infogroup.IDMS.Campaigns
{
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        #region Copy Output Export Layout
        public void CopyOrderExportLayout(int iOrderID, int iExportLayoutID, string sInitiatedBy, int campaignStatus)
        {
            try
            {
                _customCampaignRepository.CopyOrderExportLayout(iOrderID, iExportLayoutID, sInitiatedBy);
                var iHasKeyCode = _customCampaignRepository.GetAllOutputLayouts(iOrderID).FirstOrDefault(x => x.Id == iExportLayoutID).iHasKeyCode;
                var bExist = false;
                if (iHasKeyCode == 1)
                {
                    var exportLayouts = _campaignExportLayoutRepository.GetAll().Where(x => x.OrderId == iOrderID).ToList();
                    foreach (var item in exportLayouts)
                    {
                        if (item.cFieldName.Contains("tblSegment.cKeyCode1"))
                        {
                            bExist = true;
                            break;
                        }
                    }

                    if (!bExist)
                    {
                        var databaseID = _databaseRepository.GetDataSetDatabaseByOrderID(iOrderID).Id;
                        int iWidth;
                        var sWidth = _idmsConfigurationCache.GetConfigurationValue("KEYCODE1OUTPUTLENGTH", databaseID).cValue;
                        if (!int.TryParse(sWidth, out iWidth)) iWidth = 15;
                        var campaignExportLayout = new CampaignExportLayout
                        {
                            OrderId = iOrderID,
                            cFieldName = "tblSegment.cKeyCode1",
                            iExportOrder = exportLayouts.Count + 1,
                            cCalculation = string.Empty,
                            cCreatedBy = _mySession.IDMSUserName,
                            dCreatedDate = DateTime.Now,
                            iWidth = iWidth,
                            cOutputFieldName = "Key Code 1",
                            cTableNamePrefix = string.Empty,
                        };
                        _campaignExportLayoutRepository.InsertAsync(campaignExportLayout);
                    }
                }
                _orderStatusManager.UpdateOrderStatus(iOrderID, campaignStatus.Equals(Convert.ToInt32(CampaignStatus.OrderFailed)) ? CampaignStatus.OrderFailed : (campaignStatus.Equals(Convert.ToInt32(CampaignStatus.OrderCompleted))) ? CampaignStatus.OrderCompleted : (campaignStatus.Equals(Convert.ToInt32(CampaignStatus.OutputCompleted))) ? CampaignStatus.OrderCompleted : (campaignStatus.Equals(Convert.ToInt32(CampaignStatus.OutputFailed))) ? CampaignStatus.OrderCompleted:CampaignStatus.OrderCreated, sInitiatedBy);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
     
        #endregion
    }
}
