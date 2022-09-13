using System.Collections.Generic;
using System.Linq;

namespace Infogroup.IDMS.SegmentSelections.Dtos
{
    public class FieldDetails
    {
        public int ID { get; set; }
        public int iDataLength { get; set; }
        public string cFieldName { get; set; }
        public string cTableName { get; set; }
        private bool _iShowTextBox;
        public bool iShowTextBox
        {
            get
            {
                if (iShowListBox == false && iFileOperations == false)
                {
                    return true;
                }
                else
                {
                    return _iShowTextBox;
                }
            }
            set
            {
                _iShowTextBox = value;
            }
        }
        public bool iShowListBox { get; set; }
        public bool iFileOperations { get; set; }
        public int iShowDefault { get; set; }
        public string cFieldType { get; set; }
        public int iDisplayOrder { get; set; }
        public bool iIsListSpecific { get; set; }
        public string cFieldDescription { get; set; }
        public List<ValueList> values { get; set; }
        public string fileUrl { get; set; }
        
    }
    
}
