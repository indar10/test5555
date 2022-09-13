using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class GetExportLayoutSelectedFieldsDto
    {
        public int ID { get; set; }
        public int Order { get; set; }

        public string fieldName { get; set; }
        public string  OutputFieldName { get; set; }
        public string  Formula { get; set; }
        public int Width { get; set; }
         public string tablePrefix { get; set; }
        public string tableDescription { get; set; }

        public bool iIsCalculatedField { get; set; }

        public bool isFixedFields { get; set; }

        public bool isFormulaEnabled { get; set; }

        public int tableId { get; set; }

        public string fieldDescription { get; set; }
    }
}
