using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignAttachments.Dtos
{
    public class GetCampaignAttachmentForEditOutput
    {
		public CreateOrEditCampaignAttachmentDto CampaignAttachment { get; set; }

		public string CampaigncDescription { get; set;}


    }
}