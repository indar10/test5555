using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ListLoadStatuses.Dtos
{
    public class GetAllListLoadStatusesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}