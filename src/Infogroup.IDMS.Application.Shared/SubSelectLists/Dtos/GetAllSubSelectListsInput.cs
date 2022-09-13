using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.SubSelectLists.Dtos
{
    public class GetAllSubSelectListsInput : PagedAndSortedResultRequestDto
    {
		public int SubSelectId { get; set; }		 
    }
}