using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.UserSavedSelectionDetails.Dtos
{
    public class GetAllUserSavedSelectionDetailsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string UserSavedSelectioncDescriptionFilter { get; set; }

		 
    }
}