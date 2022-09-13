using Infogroup.IDMS.OrderStatuss;

namespace Infogroup.IDMS.SegmentSelections.Dtos
{

    public class SaveGlobalChangesInputDto
    {
        public int CampaignId { get; set; }
        public int DatabaseId { get; set; }
        public int DivisionId { get; set; }
        public string UserID { get; set; }
        public int SourceSegment { get; set; }
        public string Action { get; set; }
        public string TargetSegments { get; set; }
        public string SearchValue { get; set; }
        public string ReplaceValue { get; set; }
        public string FieldName { get; set; }
        public string CompareFieldName { get; set; }
        public string IncludeExclude { get; set; }
        public int Option { get; set; }
        public string FilterText { get; set; }
        public int FieldId { get; set; }
        public string FieldDescription { get; set; }
        public CampaignStatus campaignStatus { get; set; }
    }
}