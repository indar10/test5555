namespace Infogroup.IDMS.ListAutomate.Dtos
{
    public class GetIListAutomateForViewDto
    {
        public IListAutomateDto IListAutomate { get; set; }
        public int ListId { get; set; }
        public int BuildId { get; set; }
        public string LK_ListConversionFrequency { get; set; }
        public int iInterval { get; set; }
        public string cScheduleTime { get; set; }
        public string cSystemFileNameReadyToLoad { get; set; }
        public bool iIsActive { get; set; }

    }
}