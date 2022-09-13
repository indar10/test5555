using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.CampaignBillings.Dtos
{
    public class CampaignOESSDto : EntityDto
    {
		public int OrderId { get; set; }

        public int? DatabaseId { get; set; }

        public string CampaignDescription { get; set; }

        public string LK_BillingUOM { get; set; }

        public string cSalesRepID { get; set; }

        public int iBillingQty { get; set; }

        public string nUnitPrice { get; set; }

        public string nDiscountPercentage { get; set; }

        public string nEffectiveUnitPrice { get; set; }

        public string nShippingCharge { get; set; }

        public string iTotalPrice { get; set; }

        public string cCompany { get; set; }

        public string cFirstName { get; set; }

        public string cLastName { get; set; }

        public string cPhone { get; set; }

        public string cOESSInvoiceNumber { get; set; }

        public string cOESSAccountNumber { get; set; }

        public string cOracleAccountNumber { get; set; }

        public string iOESSInvoiceTotal { get; set; }

        public DateTime? dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }

        public string OESSOrderID { get; set; }

        public string OESSStatus { get; set; }

        public List<DropdownOutputDto> SalesRep { get; set; }

        public List<DropdownOutputDto> UOM { get; set; }

    }
}