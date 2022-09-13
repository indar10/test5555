using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.UserDatabases.Dtos
{
    public class GetAllUserDatabasesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string UserNameFilter { get; set; }

		 		 public string DatabasecDatabaseNameFilter { get; set; }

		 
    }
}