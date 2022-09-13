using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class GetSegmentListInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

        public int OrderId { get; set; }
    }
}