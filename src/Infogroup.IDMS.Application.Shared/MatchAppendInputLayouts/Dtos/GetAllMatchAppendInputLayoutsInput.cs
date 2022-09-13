using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.MatchAppendInputLayouts.Dtos
{
    public class GetAllMatchAppendInputLayoutsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string MatchAppendcClientNameFilter { get; set; }

		 
    }
}