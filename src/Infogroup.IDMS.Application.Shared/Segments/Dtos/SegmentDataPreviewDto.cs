using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class SegmentDataPreviewDto
    {
        public List<DropdownOutputDto> Columns { get; set; }
        public string Data { get; set; }
        public DataTable DataForExport { get; set; }
        public bool isExportLayoutCheckBoxVisible { get; set; }
        public string Description { get; set; }
        public string TooltipDescription { get; set; }
    }
}
