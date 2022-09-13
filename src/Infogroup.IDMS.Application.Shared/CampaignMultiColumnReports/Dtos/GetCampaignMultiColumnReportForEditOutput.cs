using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignMultiColumnReports.Dtos
{
    public class GetCampaignMultiColumnReportForEditOutput
    {
		public CreateOrEditCampaignMultiColumnReportDto CampaignMultiColumnReport { get; set; }

		public string CampaigncDescription { get; set;}


    }
}