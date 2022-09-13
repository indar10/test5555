using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class LoadMailerUsageDto
    {
        public TaskGeneralDto TaskGeneral;

        public int NewBuildID { get; set; }

        public string FilePath { get; set; }
    }
}
