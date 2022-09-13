
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignExportLayouts.Dtos
{
    public class CreateOrEditCampaignExportLayoutDto : EntityDto<int?>
    {

		 public int OrderId { get; set; }
		 
		 
    }
}