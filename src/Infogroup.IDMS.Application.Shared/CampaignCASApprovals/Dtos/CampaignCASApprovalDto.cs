
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.CampaignCASApprovals.Dtos
{
    public class CampaignCASApprovalDto : EntityDto
    {
        public int MasterLOLID { get; set; }
        
        public string cStatus { get; set; }

        public decimal nBasePrice { get; set; }

        public DateTime dCreatedDate { get; set; }
        
        public string cCreatedBy { get; set; }

        public string cModifiedBy { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public int OrderId { get; set; }

		 
    }
}