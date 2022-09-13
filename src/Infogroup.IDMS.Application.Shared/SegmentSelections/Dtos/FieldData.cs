namespace Infogroup.IDMS.SegmentSelections.Dtos
{
    public class FieldData
    {
        public int ID { get; set; }
        public int iDataLength { get; set; }
        public string cFieldName { get; set; }
        public string cTableName { get; set; }
        public bool iShowTextBox { get; set; }
        public bool iShowListBox { get; set; }
        public bool iFileOperations { get; set; }
        public int iShowDefault { get; set; }
        public string cFieldType { get; set; }
        public int iDisplayOrder { get; set; }
        public bool iIsListSpecific { get; set; }
        public string cFieldDescription { get; set; }
        public int iBTID { get; set; }
        public bool IsFavourite { get; set; }
        public string cDataType { get; set; }
    }
}
