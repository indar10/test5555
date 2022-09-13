using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.LoadProcessStatuses.Dtos
{
    public class GetAllLoadProcessStatusesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}