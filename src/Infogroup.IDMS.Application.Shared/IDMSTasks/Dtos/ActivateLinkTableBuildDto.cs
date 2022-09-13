using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class ActivateLinkTableBuildDto
    {
        public string TableName { get; set; }

        public string ScheduledDate { get; set; }

        public string ScheduledTime { get; set; }
    }
}
