using Infogroup.IDMS.Campaigns;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Infogroup.IDMS.CampaignBillings
{
	[Table("tblOrderBilling")]
    public class CampaignBilling : Entity 
    {
		public virtual int OrderID { get; set; }
		
		[Required]
		public virtual string LK_BillingUOM { get; set; }
		
		[Required]
		public virtual string cSalesRepID { get; set; }
		
		public virtual int iBillingQty { get; set; }
		
		public virtual decimal nUnitPrice { get; set; }
		
		public virtual decimal nDiscountPercentage { get; set; }
		
		public virtual decimal nEffectiveUnitPrice { get; set; }
		
		public virtual decimal nShippingCharge { get; set; }
		
		public virtual decimal iTotalPrice { get; set; }
		
		public virtual string cCompany { get; set; }
		
		public virtual string cFirstName { get; set; }
		
		public virtual string cLastName { get; set; }
		
		public virtual string cPhone { get; set; }
		
		public virtual string cOESSInvoiceNumber { get; set; }
		
		public virtual string cOESSAccountNumber { get; set; }
		
		public virtual string cOracleAccountNumber { get; set; }
		
		public virtual decimal? iOESSInvoiceTotal { get; set; }
		
		public virtual DateTime dCreatedDate { get; set; }
		
		[Required]
		public virtual string cCreatedBy { get; set; }
		
		public virtual DateTime? dModifiedDate { get; set; }
		
		public virtual string cModifiedBy { get; set; }
		
		public virtual int? OESSOrderID { get; set; }
		
        [ForeignKey("OrderID")]
		public Campaign CampaignFk { get; set; }
		
    }
}