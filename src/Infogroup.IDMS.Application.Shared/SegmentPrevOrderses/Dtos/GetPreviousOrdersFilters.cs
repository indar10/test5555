using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.SegmentPrevOrderses.Dtos
{
    public class GetPreviousOrdersFilters : PagedAndSortedResultRequestDto
    {
        public int CampaignId { get; set; }
        public int SegmentID { get; set; }
        public string filter { get; set; }
        public bool isFromSearch { get; set; }
    }
}