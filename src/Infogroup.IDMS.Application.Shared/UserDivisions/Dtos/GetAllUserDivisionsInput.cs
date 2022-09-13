using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.UserDivisions.Dtos
{
    public class GetAllUserDivisionsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string tblUsercFirstNameFilter { get; set; }

		 		 public string DivisioncDivisionNameFilter { get; set; }

		 
    }
}