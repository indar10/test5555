using System;
using System.ComponentModel;

namespace Infogroup.IDMS.ShippedReports.Dtos
{
    public class GetShippedReportView
    {
        [DisplayName("ID")]
        public int OrderID { get; set; }
        [DisplayName("Description")]
        public string OrderDescription { get; set; }
        [DisplayName("Database")]
        public string DatabaseName { get; set; }
        [DisplayName("Customer")]
        public string Mailer { get; set; }
        public string Broker { get; set; }
        [DisplayName("Broker PO #")]
        public string BrokerPONumber { get; set; }
        [DisplayName("PO #")]
        public string PONumber { get; set; }
        [DisplayName("Ship Date")]
        public string ShippedDate { get; set; }
        [DisplayName("Provided Qty")]
        public int ProvidedQuantity { get; set; }
     
        public string Type { get; set; }
        [DisplayName("Net Order")]
        public string NetOrder { get; set; }
        [DisplayName("No Usage")]
        public string NetUsage { get; set; }
        [DisplayName("Seed Key")]
        public string DecoyKey { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Shipped By")]
        public string ShippedBy { get; set; }

        [DisplayName("Output Layout Name")]
        public string ExportLayoutName { get; set; }
        [DisplayName("Revenue")]
        public string OESSInvoiceTotal { get; set; }
        [DisplayName("Invoice Number")]
        public string OESSInvoiceNumber { get; set; }
    }
}
