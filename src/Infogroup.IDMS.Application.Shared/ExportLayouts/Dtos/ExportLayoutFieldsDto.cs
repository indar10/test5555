using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class ExportLayoutFieldsDto
    {

        public int ID { get; set; }
        public int iExportOrder { get; set; }
        public string cOutputFieldName { get; set; }       
        public string FLDDESCR { get; set; }
        public int iWidth { get; set; }
        public bool iIsCalculatedField { get; set; }
    }
}
