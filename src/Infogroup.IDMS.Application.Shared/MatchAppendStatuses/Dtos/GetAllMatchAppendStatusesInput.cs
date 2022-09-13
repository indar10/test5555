using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.MatchAppendStatuses.Dtos
{
    public class GetAllMatchAppendStatusesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}