
using System;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.SegmentPrevOrderses.Dtos
{
    public class SegmentPrevOrdersDto : EntityDto
    {
        public int PrevOrderID { get; set; }
        public string cIncludeExclude { get; set; }
        public string cPrevSegmentID { get; set; }
        public string cPrevSegmentNumber { get; set; }
        public DateTime dCreatedDate { get; set; }
        public string cCreatedBy { get; set; }
        public DateTime? dModifiedDate { get; set; }
        public string cModifiedBy { get; set; }
        public string cCompanyFieldName { get; set; }
        public int SegmentId { get; set; }
        public ActionType action { get; set; }
        public string cMatchFieldName { get; set; }
        public string cMatchFieldDescription { get; set; }
    }
}