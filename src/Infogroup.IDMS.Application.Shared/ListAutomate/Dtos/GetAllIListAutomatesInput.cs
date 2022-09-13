using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.ListAutomate.Dtos
{
    public class GetAllIListAutomatesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}