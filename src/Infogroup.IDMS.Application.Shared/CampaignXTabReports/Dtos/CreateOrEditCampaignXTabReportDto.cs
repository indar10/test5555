
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.CampaignXTabReports.Dtos
{
    public class CreateOrEditCampaignXTabReportDto : EntityDto<int?>
    {
        public string cXField { get; set; }

        public string cYField { get; set; }

        public bool iXTabBySegment { get; set; }

        public DateTime dCreatedDate { get; set; }

        public string cCreatedBy { get; set; }

        public string cModifiedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cType { get; set; }

        public int OrderId { get; set; }

    }
}