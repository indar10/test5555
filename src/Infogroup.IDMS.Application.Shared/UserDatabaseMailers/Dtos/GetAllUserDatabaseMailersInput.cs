using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.UserDatabaseMailers.Dtos
{
    public class GetAllUserDatabaseMailersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string IDMSUsercUserIDFilter { get; set; }

		 		 public string DatabasecDatabaseNameFilter { get; set; }

		 
    }
}