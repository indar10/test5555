using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Models.Dtos
{
    public class GetAllModelsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}