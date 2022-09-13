using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class GetExportLayoutAddFieldsDto
    {
        public string FieldName { get; set; }
        public string  FieldDescription { get; set; }
        public string  FieldTableKey { get; set; }
    }
}
