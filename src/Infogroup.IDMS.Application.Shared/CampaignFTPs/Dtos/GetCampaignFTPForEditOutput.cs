using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignFTPs.Dtos
{
    public class GetCampaignFTPForEditOutput
    {
		public CreateOrEditCampaignFTPDto CampaignFTP { get; set; }


    }
}