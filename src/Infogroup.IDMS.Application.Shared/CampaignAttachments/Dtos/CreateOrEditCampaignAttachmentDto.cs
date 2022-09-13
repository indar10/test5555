
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignAttachments.Dtos
{
    public class CreateOrEditCampaignAttachmentDto : EntityDto<int?>
    {

		 public int OrderId { get; set; }
		 
		 
    }
}