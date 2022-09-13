namespace Infogroup.IDMS.SavedSelections.Dtos
{
    public class GetSavedSelectionForViewDto
    {
		public int ID { get; set;}
		public string cDescription { get; set;}
		public bool IsOR { get; set;}
		public bool UserDefault { get; set; }
        public bool iIsDefault { get; set; }
        public string cChannelType { get; set; }
    }
}