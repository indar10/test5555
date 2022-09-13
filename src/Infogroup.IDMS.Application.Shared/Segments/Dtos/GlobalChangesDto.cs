using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Segments.Dtos
{
    public class GlobalChangesDto
    {
        public PagedResultDto<SegmentsGlobalChangesDto> PagedSegments { get; set; }
        public string FinalFilterText { get; set; }
    }
}