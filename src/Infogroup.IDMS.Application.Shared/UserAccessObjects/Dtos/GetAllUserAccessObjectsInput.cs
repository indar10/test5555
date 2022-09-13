using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.UserAccessObjects.Dtos
{
    public class GetAllUserAccessObjectsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string IDMSUsercUserIDFilter { get; set; }

		 		 public string AccessObjectcCodeFilter { get; set; }

		 
    }
}