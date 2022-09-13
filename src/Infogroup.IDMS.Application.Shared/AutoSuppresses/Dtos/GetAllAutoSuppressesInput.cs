using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.AutoSuppresses.Dtos
{
    public class GetAllAutoSuppressesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string DatabasecDatabaseNameFilter { get; set; }

		 
    }
}