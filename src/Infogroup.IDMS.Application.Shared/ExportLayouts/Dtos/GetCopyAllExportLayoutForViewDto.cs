using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class GetCopyAllExportLayoutForViewDto: EntityDto
    {
        public string Description { get; set; }
    }
}
