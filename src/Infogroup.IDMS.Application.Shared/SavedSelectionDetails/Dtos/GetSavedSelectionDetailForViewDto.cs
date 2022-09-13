namespace Infogroup.IDMS.SavedSelectionDetails.Dtos
{
    public class GetSavedSelectionDetailForViewDto
    {
        public int iGroupNumber { get; set; }
        public string cJoinOperator { get; set; }
        public string cFieldDescription { get; set; }
        public string cValueOperator { get; set; }
        public string cValues { get; set; }

    }
}