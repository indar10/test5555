using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.BuildTables.Dtos
{
    public class GetAllBuildTablesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string cTableNameFilter { get; set; }


		 public string BuildcBuildFilter { get; set; }

		 
    }
}