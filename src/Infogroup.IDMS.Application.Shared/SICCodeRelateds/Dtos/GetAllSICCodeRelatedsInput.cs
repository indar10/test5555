using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.SICCodeRelateds.Dtos
{
    public class GetAllSICCodeRelatedsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}