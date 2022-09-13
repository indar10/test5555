using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class ExportEmailHygieneDataDto
    {
        public TaskGeneralDto TaskGeneral;

        public string ExportField { get; set; }

        public string ExportFlagField { get; set; }

        public string FlagValue { get; set; }
    }
}
