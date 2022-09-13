using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignCASApprovals.Dtos
{
    public class GetCampaignCASApprovalForEditOutput
    {
		public CreateOrEditCampaignCASApprovalDto CampaignCASApproval { get; set; }

		public string CampaigncDatabaseName { get; set;}


    }
}