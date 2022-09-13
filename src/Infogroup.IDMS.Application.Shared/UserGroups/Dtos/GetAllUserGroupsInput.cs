using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.UserGroups.Dtos
{
    public class GetAllUserGroupsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string TblUsercFirstNameFilter { get; set; }

		 
    }
}