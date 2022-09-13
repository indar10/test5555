using Abp.UI;
using Infogroup.IDMS.CampaignBillings;
using Infogroup.IDMS.CampaignBillings.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Infogroup.IDMS.Campaigns
{
    public partial class CampaignsAppService : IDMSAppServiceBase, ICampaignsAppService
    {
        #region OESS Tab
        private CampaignOESSDto FetchOESSDetails(int campaignId, int iProvidedQty)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var oessDetails = new CampaignOESSDto();
                var oessData = _campaignBillingRepository.GetAll().FirstOrDefault(x => x.OrderID == campaignId);
                if (oessData != null)
                    oessDetails = CommonHelpers.ConvertNullStringToEmptyAndTrim(ObjectMapper.Map<CampaignOESSDto>(oessData));
                else
                {
                    oessDetails = CommonHelpers.ConvertNullStringToEmptyAndTrim(oessDetails);
                    oessDetails.OrderId = campaignId;
                }

                if (string.IsNullOrEmpty(oessDetails.cSalesRepID))
                {
                    var cSalesRepID = GetSalesRepDropdownValues(_mySession.IDMSUserId);
                    oessDetails.cSalesRepID = cSalesRepID.Count > 0 ? Convert.ToString(cSalesRepID[0].Value) : "";
                }
                oessDetails.OESSStatus = FetchOESSStatus(campaignId);
                oessDetails.UOM = GetUOMDropdownValues();
                oessDetails.SalesRep = GetSalesRepDropdownValues();
                if (string.IsNullOrEmpty(oessDetails.LK_BillingUOM))
                    oessDetails.LK_BillingUOM = Convert.ToString(oessDetails.UOM[0].Value);
                oessDetails.nUnitPrice = string.IsNullOrEmpty(oessDetails.nUnitPrice) || oessDetails.nUnitPrice == "0.000" ? "" : oessDetails.nUnitPrice;
                oessDetails.OESSOrderID = string.IsNullOrEmpty(oessDetails.OESSOrderID) || Convert.ToInt32(oessDetails.OESSOrderID) == 0 ? "-" : oessDetails.OESSOrderID;
                oessDetails.cCompany = string.IsNullOrEmpty(oessDetails.cCompany) ? "-" : oessDetails.cCompany;
                oessDetails.cFirstName = string.IsNullOrEmpty(oessDetails.cFirstName) ? "-" : oessDetails.cFirstName;
                oessDetails.cLastName = string.IsNullOrEmpty(oessDetails.cLastName) ? "-" : oessDetails.cLastName;
                oessDetails.cPhone = string.IsNullOrEmpty(oessDetails.cPhone) ? "-" : string.Format("{0:(###) ###-####}", Convert.ToInt64(oessData.cPhone));
                oessDetails.cOESSInvoiceNumber = string.IsNullOrEmpty(oessDetails.cOESSInvoiceNumber) ? "-" : oessDetails.cOESSInvoiceNumber;
                oessDetails.cOESSAccountNumber = string.IsNullOrEmpty(oessDetails.cOESSAccountNumber) ? "-" : oessDetails.cOESSAccountNumber;
                oessDetails.cOracleAccountNumber = string.IsNullOrEmpty(oessDetails.cOracleAccountNumber) ? "-" : oessDetails.cOracleAccountNumber;
                oessDetails.iOESSInvoiceTotal = string.IsNullOrEmpty(oessDetails.iOESSInvoiceTotal) || oessDetails.iOESSInvoiceTotal == "0.00" ? "-" : oessDetails.iOESSInvoiceTotal;
                oessDetails.iBillingQty = oessDetails.iBillingQty == 0 ? iProvidedQty : oessDetails.iBillingQty;
                sw.Stop();
                Logger.Info($"\r\n ----- For campaignId:{campaignId}, Total time for FetchOESSDetails: {sw.Elapsed.TotalSeconds} ----- \r\n");

                return oessDetails;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private List<DropdownOutputDto> GetUOMDropdownValues()
        {
            return _lookupCache.GetLookUpFields("BILLINGUOM")
            .Select(t => new DropdownOutputDto
            {
                Value = t.cCode.ToString(),
                Label = t.cDescription
            }).ToList();

        }

        private List<DropdownOutputDto> GetSalesRepDropdownValues(int userId = 0)
        {
            return _customCampaignRepository.GetSalesRepDropdownValues(userId);

        }


        private string FetchOESSStatus(int campaignId)
        {
            var OESSStatus = _customCampaignRepository.GetOESSStatusByCampaignID(campaignId);
            CampaignOESSConsts oessEnum = (CampaignOESSConsts)OESSStatus;
            var statusName = string.Empty;
            switch (oessEnum)
            {
                case CampaignOESSConsts.Saved:
                    statusName = CampaignOESSDescription.Saved;
                    break;
                case CampaignOESSConsts.SubmittedToCredit:
                    statusName = CampaignOESSDescription.SubmittedToCredit;
                    break;
                case CampaignOESSConsts.ApprovedByCredit:
                    statusName = CampaignOESSDescription.ApprovedByCredit;
                    break;
                case CampaignOESSConsts.RejectedByCredit:
                    statusName = CampaignOESSDescription.RejectedByCredit;
                    break;
                case CampaignOESSConsts.Invoiced:
                    statusName = CampaignOESSDescription.Invoiced;
                    break;
                case CampaignOESSConsts.New:
                    statusName = CampaignOESSDescription.New;
                    break;
                default:
                    break;
            }
            return statusName;
        }


        public string LaunchOESS(CampaignOESSDto input)
        {
            CampaignBilling oessData;
            if (input.Id == 0)
            {
                oessData = new CampaignBilling()
                {
                    cCreatedBy = _mySession.IDMSUserName,
                    dCreatedDate = DateTime.Now
                };
            }
            else
            {
                oessData = _campaignBillingRepository.GetAll().FirstOrDefault(x => x.OrderID == input.OrderId);

                oessData.cModifiedBy = _mySession.IDMSUserName;
                oessData.dModifiedDate = DateTime.Now;
            }

            oessData.OrderID = Convert.ToInt32(input.OrderId);
            oessData.LK_BillingUOM = string.IsNullOrEmpty(input.LK_BillingUOM)?string.Empty: input.LK_BillingUOM;
            oessData.cSalesRepID = string.IsNullOrEmpty(input.cSalesRepID)?string.Empty: input.cSalesRepID;
            oessData.iBillingQty = Convert.ToInt32(input.iBillingQty);
            oessData.nUnitPrice = string.IsNullOrEmpty(input.nUnitPrice)?0:Convert.ToDecimal(input.nUnitPrice);
            input.nDiscountPercentage = !string.IsNullOrEmpty(input.nDiscountPercentage) ? input.nDiscountPercentage : "0";
            input.nShippingCharge = !string.IsNullOrEmpty(input.nShippingCharge) ? input.nShippingCharge : "0";
            oessData.nDiscountPercentage = string.IsNullOrEmpty(input.nDiscountPercentage)?0:Convert.ToDecimal(input.nDiscountPercentage);
            oessData.nEffectiveUnitPrice = string.IsNullOrEmpty(input.nEffectiveUnitPrice)?0:Convert.ToDecimal(input.nEffectiveUnitPrice);
            oessData.nShippingCharge = string.IsNullOrEmpty(input.nShippingCharge)?0:Convert.ToDecimal(input.nShippingCharge);
            oessData.iTotalPrice = string.IsNullOrEmpty(input.iTotalPrice)?0:Convert.ToDecimal(input.iTotalPrice);

            if (input.Id == 0)
            {
                _campaignBillingRepository.Insert(oessData);
            }

            CurrentUnitOfWork.SaveChanges();

            // Encode the key and redirect to external url 
            var prodCode = _idmsConfigurationCache.GetConfigurationValue("OESSProductCode", Convert.ToInt32(input.DatabaseId)).cValue;
            var url = $"{_idmsConfigurationCache.GetConfigurationValue("OESSBaseURL", 0).cValue}&Key=";
            var passPhrase = _idmsConfigurationCache.GetConfigurationValue("OESSPassPhrase", 0).cValue;
            var sQueryStringOESS = $"repid={input.cSalesRepID}&ordername={input.CampaignDescription}&prodcode={prodCode}&qty={input.iBillingQty}&shipamount={input.nShippingCharge}&relatedorderid={input.OrderId}&unitprice={input.nEffectiveUnitPrice}&uom={input.LK_BillingUOM}";
            var encodedUrl = System.Net.WebUtility.UrlEncode(_utils.Encrypt(sQueryStringOESS, passPhrase));
            var completeUrl = string.Concat(url, encodedUrl.ToString());

            return completeUrl;
        }

    }

    #endregion
}
