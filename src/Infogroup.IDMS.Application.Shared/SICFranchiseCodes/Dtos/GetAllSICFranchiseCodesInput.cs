using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.SICFranchiseCodes.Dtos
{
    public class GetAllSICFranchiseCodesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}