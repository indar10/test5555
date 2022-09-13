using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infogroup.IDMS.CampaignXTabReports.Dtos;
using System.Collections.Generic;
using Infogroup.IDMS.CampaignMultiColumnReports.Dtos;
using Infogroup.IDMS.CampaignAttachments.Dtos;
using Infogroup.IDMS.CampaignMaxPers.Dtos;
using Infogroup.IDMS.Decoys.Dtos;
using Infogroup.IDMS.CampaignBillings.Dtos;

namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class CreateOrEditCampaignDto : EntityDto<int?>
    {
        public CampaignGeneralDto GeneralData { get; set; }

        public int CurrentStatus;
        public List<GetCampaignXTabReportsListForView> listOfXTabRecords { get; set; }

        public List<GetCampaignMultidimensionalReportForViewDto> listOfMultidimensionalRecords { get; set; }

        public EditCampaignsOutputDto campaignOutputDto { get; set; }

        public GetCampaignsOutputDto getOutputData { get; set; }

        public GetXtabReportsDataDto reportsData { get; set; }

        public List<CampaignAttachmentDto> documentsData { get; set; }

        public GetMultidimensionalReportsDataDto multiReportsData { get; set; }
        public CampaignBillingDto BillingData { get; set; }
        public GetCampaignMaxPerForViewDto maxPerData {get;set;}
        public GetDecoyForViewDto decoyData { get; set; }

        public Boolean isFavouriteCampaign { get; set; }
        public int divisionMailerId { get; set; }
        public int divisionBrokerId { get; set; }

        public CampaignOESSDto OESSData { get; set; }
        public bool IsStatusChangeRequired { get; set; } = true;

        public bool IsChannelTypeVisible { get; set; }
        public int DocumentFileSize { get; set; }

    }
    public class OrderStatusDto
    {

        public int OrderID { get; set; }

        public int iStatus { get; set; }

        public bool iIsCurrent { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string cNotes { get; set; }

        public DateTime dCreatedDate { get; set; }

        [Required]
        [StringLength(25)]
        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        [StringLength(25)]
        public string cModifiedBy { get; set; }

        public bool? iStopRequested { get; set; }
    }

}