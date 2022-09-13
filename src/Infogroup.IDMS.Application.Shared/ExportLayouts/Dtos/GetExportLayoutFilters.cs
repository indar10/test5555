using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class GetExportLayoutFilters : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
       

    }
}
