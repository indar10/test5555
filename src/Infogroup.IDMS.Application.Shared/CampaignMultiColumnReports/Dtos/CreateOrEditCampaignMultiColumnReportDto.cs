
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignMultiColumnReports.Dtos
{
    public class CreateOrEditCampaignMultiColumnReportDto : EntityDto<int?>
    {

		 public int OrderId { get; set; }
		 
		 
    }
}