﻿using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.MatchAppendDatabaseUsers.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}