using Abp.Application.Services.Dto;
namespace Infogroup.IDMS.SegmentSelections.Dtos
{

    public class GetGlobalChangesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int OrderId { get; set; }
        public bool isFindReplace { get; set; }
    }
}