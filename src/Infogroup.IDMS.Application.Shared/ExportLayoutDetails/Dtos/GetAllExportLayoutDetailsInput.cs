using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ExportLayoutDetails.Dtos
{
    public class GetAllExportLayoutDetailsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string ExportLayoutcDescriptionFilter { get; set; }

		 
    }
}