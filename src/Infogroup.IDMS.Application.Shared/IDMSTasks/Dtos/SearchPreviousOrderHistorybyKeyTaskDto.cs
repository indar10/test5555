using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class SearchPreviousOrderHistorybyKeyTaskDto
    {
        public TaskGeneralDto TaskGeneral;

        public string SearchKey { get; set; }

        public int CampaignId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
