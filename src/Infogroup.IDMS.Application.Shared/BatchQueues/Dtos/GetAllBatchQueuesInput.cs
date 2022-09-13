using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.BatchQueues.Dtos
{
    public class GetAllBatchQueuesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}