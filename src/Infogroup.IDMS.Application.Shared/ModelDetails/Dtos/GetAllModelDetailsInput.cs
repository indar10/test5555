using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ModelDetails.Dtos
{
    public class GetAllModelDetailsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}