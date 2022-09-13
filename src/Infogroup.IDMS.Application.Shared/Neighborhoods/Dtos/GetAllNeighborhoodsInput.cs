using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Neighborhoods.Dtos
{
    public class GetAllNeighborhoodsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}