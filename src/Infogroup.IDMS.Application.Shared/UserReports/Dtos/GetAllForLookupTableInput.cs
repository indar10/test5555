﻿using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserReports.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}