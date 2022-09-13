using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class GetFastCountCampaignListFilters: PagedAndSortedResultRequestDto
    {       
        public string OrderId { get; set; }       
        public List<string> Status { get; set; }
        public List<string> DatabaseId { get; set; }
        public List<string> UserId { get; set; }
        public DateTime[] selectedDateRange { get; set; }   
    }
}
