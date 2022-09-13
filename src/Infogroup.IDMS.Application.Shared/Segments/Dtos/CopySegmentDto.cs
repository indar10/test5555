using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class CopySegmentDto: EntityDto<int>
    {
        public int iSegmentId { get; set; }
        public int iCampaignId { get; set; }
        public int iMaxDedupeId { get; set; }
        public int iCopyToOrderID { get; set; }
        public int iCopyFromOrderID { get; set; }
        public int iSegmentFrom { get; set; }
        public int iSegmentTo { get; set; }
        public string sInitiatedBy { get; set; }
        public string sCommaSeparatedSegments { get; set; }
        public string cCopyMode { get; set; }        
        public int iNumberOfCopies { get; set; }
        public string cSegmentDescription { get; set; }
        public string cKeyCode1 { get; set; }
        public string cKeyCode2 { get; set; }
        public string cmaxPer { get; set; }
        public int? iGroup { get; set; }
        public int? iRequiredQty { get; set; }
    }
}
