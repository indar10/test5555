using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.MasterLoLs.Dtos
{
    public  class ExportListMailerAccess : PagedAndSortedResultRequestDto
    {
        public int ListId { get; set; }
        public string Code { get; set; }
        public string ListName { get; set; }
        public string Type { get; set; }
        public string Company { get; set; }

    }
}
