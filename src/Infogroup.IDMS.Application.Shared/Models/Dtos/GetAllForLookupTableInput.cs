﻿using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Models.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}