using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.MatchAppendOutputLayouts.Dtos
{
    public class GetAllMatchAppendOutputLayoutsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}