using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.CampaignXTabReports.Dtos
{
    public class GetAllCampaignXTabReportsInput : PagedAndSortedResultRequestDto
    {
		 public int campaignID { get; set; }
    }
}