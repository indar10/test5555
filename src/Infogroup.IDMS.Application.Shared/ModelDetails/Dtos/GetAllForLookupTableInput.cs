﻿using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.ModelDetails.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}