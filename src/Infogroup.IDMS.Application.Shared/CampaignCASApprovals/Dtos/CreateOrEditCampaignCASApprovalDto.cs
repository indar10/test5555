
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignCASApprovals.Dtos
{
    public class CreateOrEditCampaignCASApprovalDto : EntityDto<int?>
    {

		 public int OrderId { get; set; }
		 
		 
    }
}