using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.DivisionMailers.Dtos
{
    public class GetAllDivisionMailersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
        public string IsActive { get; set; }
    }
}