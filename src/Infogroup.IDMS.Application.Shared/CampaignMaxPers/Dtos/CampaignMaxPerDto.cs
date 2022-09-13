
using System;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.CampaignMaxPers.Dtos
{
    public class SegmentLevelMaxPerDto : EntityDto
    {        
        public string cGroup { get; set; }

        public string cMaxPerField { get; set; }
        public string cMaxPerFieldDescription { get; set; }

        public int? iMaxPerQuantity { get; set; }

        public DateTime dCreatedDate { get; set; }
     
        public string cCreatedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public string cModifiedBy { get; set; }
        public int OrderId { get; set; }
        public ActionType SegmentLevelAction;
    }
    public class CampaignLevelMaxPerDto:EntityDto
    {
        public int cMinimumQuantity { get; set; }
        public int cMaximumQuantity { get; set; }
        public string cMaxPerFieldOrderLevel { get; set; }
    }
}