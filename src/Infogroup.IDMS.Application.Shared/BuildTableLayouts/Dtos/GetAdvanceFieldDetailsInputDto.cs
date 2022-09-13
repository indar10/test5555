namespace Infogroup.IDMS.BuildTableLayouts.Dtos
{

    public class GetAdvanceFieldDetailsInputDto
    {
        public int DatabaseId { get; set; }
        public int BuildId { get; set; }
        public AdvanceSelectionScreen Screen { get; set; }

    }
}