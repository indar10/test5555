using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.AccessObjects.Dtos
{
    public class GetAllAccessObjectsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}