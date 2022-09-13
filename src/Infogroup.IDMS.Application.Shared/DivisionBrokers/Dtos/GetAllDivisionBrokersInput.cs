using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.DivisionBrokers.Dtos
{
    public class GetAllDivisionBrokersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}