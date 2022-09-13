using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.MatchAppends.Dtos
{
    public class GetAllMatchAppendsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
        public string UserNameFiler { get; set; }
    }
}