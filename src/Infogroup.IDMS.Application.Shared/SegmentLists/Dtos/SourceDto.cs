using Abp.Application.Services.Dto;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.SegmentLists.Dtos
{
    public class SourceDto : EntityDto
    {
        public string ListName { get; set; }
        public int ListID { get; set; }
        public ActionType Action { get; set; }    
    }
}