using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ModelQueues.Dtos
{
    public class GetAllModelQueuesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}