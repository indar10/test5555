using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class CampaignQueueDto
    {
        public int id { get; set; }

        public string cdescription { get; set; }

        public string dcreateddate { get; set; }

        public string cCreatedBy { get; set; }

        public string iStatus { get; set; }
        public int StatusNumber { get; set; }

        public string cModifiedBy { get; set; }

        public string dModifiedDate { get; set; }

        public string cDivisionName { get; set; }
        public int DatabaseId { get; set; }

        public string iStopRequested { get; set; }

        public string cDatabaseName { get; set; }

        public bool IsLocked { get; set; }

    }
}
