using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.SecurityGroups.Dtos
{
    public class GetAllSecurityGroupsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

        public int SelectedDatabase { get; set; }
    }
}