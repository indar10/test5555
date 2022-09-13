using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.DivisionShipTos.Dtos
{
    public class GetAllDivisionShipTosInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public bool iIsActiveFilter { get; set; }
    }
}