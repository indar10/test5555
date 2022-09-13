using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ExportLayouts.Dtos
{
    public class GetAllExportLayoutsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string DatabasecDatabaseNameFilter { get; set; }

		 
    }
}