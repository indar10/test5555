using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class LayoutTemplateDto
    {
        public int Order { get; set; }
        public string FieldName { get; set; }
        public string Formula { get; set; }

        public string Width { get; set; }
    }
}
