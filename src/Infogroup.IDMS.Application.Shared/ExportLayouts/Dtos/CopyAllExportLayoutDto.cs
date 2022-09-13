using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class CopyAllExportLayoutDto
    {
        public int DatabaseFromId { get; set; }

        public int GroupFromId { get; set; }

        public int DatabaseToId { get; set; }

        public int GroupToId { get; set; }

        public List<GetCopyAllExportLayoutForViewDto> Layouts { get; set; }
    }
}
