using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.UserDatabaseAccessObjects.Dtos
{
    public class GetAllUserDatabaseAccessObjectsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string IDMSUsercFirstNameFilter { get; set; }

		 		 public string AccessObjectcCodeFilter { get; set; }

		 		 public string DatabasecDatabaseNameFilter { get; set; }

		 
    }
}