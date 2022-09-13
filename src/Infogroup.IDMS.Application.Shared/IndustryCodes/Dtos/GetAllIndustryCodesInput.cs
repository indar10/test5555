using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.IndustryCodes.Dtos
{
    public class GetAllIndustryCodesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}