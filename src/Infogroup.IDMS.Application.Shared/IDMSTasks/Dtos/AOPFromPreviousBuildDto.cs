using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class AOPFromPreviousBuildDto
    {
        public TaskGeneralDto TaskGeneral;

        public int NewBuildID { get; set; }

        public string ListId { get; set; }

        public bool userAgree { get; set; }

        public bool isRegister { get; set; }
    }
}
