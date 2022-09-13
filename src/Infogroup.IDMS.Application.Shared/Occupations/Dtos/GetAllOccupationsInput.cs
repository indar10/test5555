using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Occupations.Dtos
{
    public class GetAllOccupationsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}