namespace Infogroup.IDMS.SegmentSelections.Dtos
{

    public class FindReplaceInputDto
    {
        public string Filter { get; set; }
        public int CampaignId { get; set; }
        public int BuildId { get; set; }
        public int DatabaseId { get; set; }
        public string FindText { get; set; }
        public string ReplaceText { get; set; }
        public int FieldId { get; set; }
        public string cQuestionFieldName { get; set; }
        public string cQuestionDescription { get; set; }
    }
}