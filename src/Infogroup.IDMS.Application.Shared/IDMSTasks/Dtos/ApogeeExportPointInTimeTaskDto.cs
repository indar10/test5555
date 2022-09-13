using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class ApogeeExportPointInTimeTaskDto
    {
        public TaskGeneralDto TaskGeneral;

        public int BuildID { get; set; }

        public string FileLocation { get; set; }

        public string InputFileName { get; set; }
    }
}
