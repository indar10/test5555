using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class GetAllExportLayoutForCopyDto
    {
        public int DatabaseId { get; set; }

        public int GroupId { get; set; }

        public string Sorting { get; set; }
    }
}
