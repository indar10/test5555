namespace Infogroup.IDMS.BuildTableLayouts.Dtos
{
    public class GetAllBuildTableLayoutsInput
    {
		public string Filter { get; set; }
        public int BuildId { get; set; }
        public int DatabaseId { get; set; }
        public int MailerId { get; set; }
    }
}