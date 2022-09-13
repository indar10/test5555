using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class ImportEmailHygieneDataDto
    {
        public TaskGeneralDto TaskGeneral;

        public string MatchField { get; set; }

        public string FlagField { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }
    }
}
