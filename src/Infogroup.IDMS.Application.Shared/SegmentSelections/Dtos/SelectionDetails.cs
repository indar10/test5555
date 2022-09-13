namespace Infogroup.IDMS.SegmentSelections.Dtos
{
    public class SelectionDetails
    {
        public int ID { get; set; }
        public string cValueMode { get; set; }
        public string IsZipRadius { get; set; }
        public string cFieldType { get; set; }
        public string cSystemFileName { get; set; }
        public int FieldID { get; set; }
        public string cQuestionFieldName { get; set; }
        public string cQuestionDescription { get; set; }
        public string cGrouping { get; set; }
        public bool iShowTextBox { get; set; }
        public bool iShowListBox { get; set; }
        public bool iFileOperations { get; set; }
        public int iGroupNumber { get; set; }
        public string cJoinOperator { get; set; }
        public string Description { get; set; }
        public string cDescription { get; set; }
        public string cValueOperator { get; set; }
        public bool iIsListSpecific { get; set; }
        public bool iIsRAWNotMapped { get; set; }
        public string cValues { get; set; }
        public string fn { get; set; }
        public string ext { get; set; }
        public string clfn { get; set; }
        public string cTableName { get; set; }
        public string cFileName { get; set; }
    }
}
