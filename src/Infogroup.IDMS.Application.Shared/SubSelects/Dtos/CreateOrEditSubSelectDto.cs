
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Infogroup.IDMS.SubSelects.Dtos
{
    public class CreateOrEditSubSelectDto : EntityDto<int>
    {
        public string cIncludeExclude { get; set; }
        public string cCompanyIndividual { get; set; }
        public DateTime dCreatedDate { get; set; }
        public string cCreatedBy { get; set; }
        public DateTime? dModifiedDate { get; set; }
        public string cModifiedBy { get; set; }   
        public int SegmentId { get; set; }
        public int CampaignId { get; set; }
        // Linked Sources
        public List<int> SourceIds { get; set; }
        public List<int> deletedSourceIds { get; set; }
    }
}