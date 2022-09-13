using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.UserSavedSelections.Dtos
{
    public class GetAllUserSavedSelectionsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string DatabasecDatabaseNameFilter { get; set; }

		 
    }
}