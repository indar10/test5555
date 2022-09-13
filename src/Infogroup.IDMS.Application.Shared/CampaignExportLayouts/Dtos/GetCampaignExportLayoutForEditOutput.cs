using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignExportLayouts.Dtos
{
    public class GetCampaignExportLayoutForEditOutput
    {
		public CreateOrEditCampaignExportLayoutDto CampaignExportLayout { get; set; }

		public string CampaigncDescription { get; set;}


    }
}