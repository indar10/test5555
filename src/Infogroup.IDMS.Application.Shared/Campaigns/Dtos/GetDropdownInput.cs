namespace Infogroup.IDMS.Campaigns.Dtos
{
    public class GetDropdownInput 
    {
		public string Filter { get; set; }
        public int DatabaseID { get; set; }
        public bool DivisionalDatabase { get; set; }
    }
}