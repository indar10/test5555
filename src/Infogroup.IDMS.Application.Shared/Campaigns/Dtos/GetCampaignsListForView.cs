using System;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class GetCampaignsListForView
    {
        public int BuildID { get; set; }
        public string BuildDescription { get; set; }
        public string OrderCreatedDate { get; set; }
        public int Status { get; set; }
        public int CampaignId { get; set; }
        public int iAvailableQty { get; set; }
        public string CampaignDescription { get; set; }
        public decimal ProvidedQty { get; set; }
        public int MailerId { get; set; }
        public int OfferID { get; set; }
        public string OfferName { get; set; }
        public string CreatedBy { get; set; }
        public string Mailer { get; set; }
        public string DatabaseName { get; set; }
        public int DatabaseID { get; set; }
        public int DivisionId { get; set; }
        public int SegmentID { get; set; }
        public int SplitType { get; set; }
        public string StatusDescription { get; set; }
        public string CustomerDescription { get; set; }
        public int Build { get; set; }
        public bool IsLocked { get; set; }
        public string cExportLayout { get; set; }
        public string PoOrderNumber { get; set; }
    }

}
