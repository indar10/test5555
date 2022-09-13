using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.MasterLoLs.Dtos
{
    public class GetAllMasterLoLsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
		public string DatabaseID { get; set; }

		 
    }
}