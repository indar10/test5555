using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class OptoutHardbounceTaskDto
    {
        public TaskGeneralDto TaskGeneral;

        public string FileType { get; set; }

        public string FileLocation { get; set; }

        public string InputFileName { get; set; }
    }
}
