using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ProcessQueueDatabases.Dtos
{
    public class dropdownForProcessQueueDto : PagedAndSortedResultRequestDto
    {
        public int value { get; set; }
        public string label  { get; set; }

        public string action { get; set; }
    }
}
