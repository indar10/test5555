using Abp.Application.Services.Dto;
using System;

namespace Infogroup.IDMS.CampaignFTPs.Dtos
{
    public class GetAllCampaignFTPsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}