using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.SelectionFieldCountReports.Dtos
{
    public class GetOrderDetailsView
    {
        public int OrderId { get; set; }
        public string cDescription { get; set; }
        public int iProvidedCount { get; set; }        
        public string dCreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
