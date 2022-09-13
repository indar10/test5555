using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class GetCampaignListFilters : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public string ID { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime[] selectedDateRange { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string DatabaseName { get; set; }
        public string CustomerName { get; set; }
        public string BuildDescription { get; set; }
    }
}
