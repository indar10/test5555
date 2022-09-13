using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class SetValidEmailFlagDto
    {
        public TaskGeneralDto TaskGeneralFrom;


        public TaskGeneralDto TaskGeneralTo;

        public string ScheduledDate { get; set; }

        public string ScheduledTime { get; set; }

    }
}
