using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Segments.Dtos
{

    public class SegmentsGlobalChangesDto : EntityDto<int>
    {
        public string cDescription { get; set; }
        public string cKeyCode1 { get; set; }
        public int iRequiredQty { get; set; }
        public int iProvidedQty { get; set; }
        public int iDedupeOrderSpecified { get; set; }
        public int iAvailableQty { get; set; }
    }
}