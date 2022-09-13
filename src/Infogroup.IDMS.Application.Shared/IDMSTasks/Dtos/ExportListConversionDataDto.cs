using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.IDMSTasks.Dtos
{
    public class ExportListConversionDataDto
    {
        public TaskGeneralDto TaskGeneral;

        public string ListId { get; set; }

        public int TableType { get; set; }

        public string Fields { get; set; }

        public string OutputType { get; set; }

        public int OutputQuantity { get; set; }

        public string Selection { get; set; }
    }
}
