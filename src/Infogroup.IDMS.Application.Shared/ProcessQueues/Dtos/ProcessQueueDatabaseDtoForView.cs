using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infogroup.IDMS.ProcessQueues.Dtos
{
   public class ProcessQueueDatabaseDtoForView : PagedAndSortedResultRequestDto
    {
        public int databaseId { get; set; }
        public int processQueueId { get; set; }

        public string cDatabaseName { get; set; }
    }
}
