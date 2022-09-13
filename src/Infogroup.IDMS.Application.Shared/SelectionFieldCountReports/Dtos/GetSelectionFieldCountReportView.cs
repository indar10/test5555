using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Infogroup.IDMS.SelectionFieldCountReports.Dtos
{
    public class GetSelectionFieldCountReportView
    {
        [DisplayName("Field Name")]
        public string cQuestionFieldName { get; set; }
        [DisplayName("Status")]
        public string cDescription { get; set; }
        [DisplayName("# of Times Used")]
        public int count { get; set; }
        public int iStatus { get; set; }
        public List<GetOrderDetailsView> OrderDetailsList { get; set; }
    }
}
