using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class EnhancedAuditReportDto
    {
        public TaskGeneralDto TaskGeneral;

        public string CountId { get; set; }

        public bool IsListWiseReport { get; set; }

        public string ScheduledDate { get; set; }

        public string ScheduledTime { get; set; }
    }
}
