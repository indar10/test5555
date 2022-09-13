using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class ExportLayoutTemplateDto
    {
        public string DatabaseName { get; set; }

        public string Description { get; set; }
        public string GroupName { get; set; }
        public string Telemarketing { get; set; }
        public string OutputCase { get; set; }


        public string Order { get; set; }
        public string OutputFieldName { get; set; }
        public string Formula { get; set; }
        public string Width { get; set; }
        public string TableName { get; set; }
        public string TableDescription { get; set; }
    }
}
